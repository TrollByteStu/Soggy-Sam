using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif


namespace Water2DTool {
    [ExecuteAlways]
    public class Water2D_PlanarReflection : MonoBehaviour
    {
        [SerializeField]
        [Range(1, 10)]
        private int reflectionQuality = 10;
        public LayerMask m_ReflectLayers = -1;
        public float m_ClipPlaneOffset = 0.0f;
        public float reflectionRenderDistance = 1000;

        private static Camera m_ReflectionCamera;
        private RenderTexture m_ReflectionTexture = null;
        private Water2D_Tool water2D;
        private string reflectionSampler = "_ReflectionTex";
        private Material m_SharedMaterial;


        private void Awake()
        {
            water2D = GetComponentInParent<Water2D_Tool>();
        }

        void Start()
        {
            gameObject.layer = LayerMask.NameToLayer("Water");
            setMaterial();
        }

        private void OnEnable()
        {
            gameObject.layer = LayerMask.NameToLayer("Water");
            RenderPipelineManager.beginCameraRendering += ExecutePlanarReflections;
            setMaterial();
        }

        // Cleanup all the objects we possibly have created
        private void OnDisable()
        {
            Cleanup();
        }

        private void OnDestroy()
        {
            Cleanup();
        }

        void Cleanup()
        {
            RenderPipelineManager.beginCameraRendering -= ExecutePlanarReflections;

            if (m_ReflectionCamera)
            {
                m_ReflectionCamera.targetTexture = null;
                SafeDestroy(m_ReflectionCamera.gameObject);
            }

            if (m_ReflectionTexture)
            {
                RenderTexture.ReleaseTemporary(m_ReflectionTexture);
            }
        }

        void SafeDestroy(Object obj)
        {
            if (Application.isEditor)
            {
                DestroyImmediate(obj);
            }
            else
            {
                Destroy(obj);
            }
        }

        public void setMaterial()
        {
            if (Application.isPlaying)
                m_SharedMaterial = GetComponent<Renderer>().material;
            else
                m_SharedMaterial = GetComponent<Renderer>().sharedMaterial;

        }

        private void UpdateCamera(Camera src, Camera dest)
        {
            if (dest == null)
                return;
            dest.CopyFrom(src);
            dest.useOcclusionCulling = false;
        }

        private void UpdateReflectionCamera(Camera realCamera)
        {
            if (m_ReflectionCamera == null)
                m_ReflectionCamera = CreateMirrorObjects(realCamera);

            Vector3 pos = Vector3.zero;
            Vector3 normal = Vector3.up;

            Transform reflectiveSurface = transform; //waterHeight;

            Vector3 eulerA = realCamera.transform.eulerAngles;

            m_ReflectionCamera.transform.eulerAngles = new Vector3(-eulerA.x, eulerA.y, eulerA.z);
            m_ReflectionCamera.transform.position = realCamera.transform.position;

            if (water2D == null)
                water2D = GetComponentInParent<Water2D_Tool>();

            if (water2D != null)
                pos = transform.TransformPoint(water2D.handlesPosition[0]);
            else
                pos = transform.position;

            normal = reflectiveSurface.transform.up;

            UpdateCamera(realCamera, m_ReflectionCamera);

            float d = -Vector3.Dot(normal, pos) - m_ClipPlaneOffset;
            Vector4 reflectionPlane = new Vector4(normal.x, normal.y, normal.z, d);

            Matrix4x4 reflection = Matrix4x4.zero;

            CalculateReflectionMatrix(ref reflection, reflectionPlane);
            Vector3 oldpos = realCamera.transform.position;
            Vector3 newpos = ReflectPosition(oldpos);

            m_ReflectionCamera.worldToCameraMatrix = realCamera.worldToCameraMatrix * reflection;

            Vector4 clipPlane = CameraSpacePlane(m_ReflectionCamera, pos, normal, 1.0f);
            Matrix4x4 projection = realCamera.projectionMatrix;
            projection = CalculateObliqueMatrix(projection, clipPlane);

            m_ReflectionCamera.projectionMatrix = projection;
            m_ReflectionCamera.cullingMask = m_ReflectLayers; // never render water layer
            m_ReflectionCamera.transform.position = newpos;

            Vector3 euler = realCamera.transform.eulerAngles;

            m_ReflectionCamera.transform.eulerAngles = new Vector3(-euler.x, euler.y, euler.z);
        }

        // Given position/normal of the plane, calculates plane in camera space.
        private Vector4 CameraSpacePlane(Camera cam, Vector3 pos, Vector3 normal, float sideSign)
        {
            Vector3 offsetPos = pos + normal * m_ClipPlaneOffset;
            Matrix4x4 m = cam.worldToCameraMatrix;
            Vector3 cpos = m.MultiplyPoint(offsetPos);
            Vector3 cnormal = m.MultiplyVector(normal).normalized * sideSign;

            return new Vector4(cnormal.x, cnormal.y, cnormal.z, -Vector3.Dot(cpos, cnormal));
        }

        private Camera CreateMirrorObjects(Camera currentCamera)
        {
            GameObject go = new GameObject($"Planar Refl Camera id{GetInstanceID().ToString()} for {currentCamera.GetInstanceID().ToString()}", typeof(Camera));
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData lwrpCamData = go.AddComponent(typeof(UnityEngine.Rendering.Universal.UniversalAdditionalCameraData)) as UnityEngine.Rendering.Universal.UniversalAdditionalCameraData;
            UnityEngine.Rendering.Universal.UniversalAdditionalCameraData lwrpCamDataCurrent = currentCamera.GetComponent<UnityEngine.Rendering.Universal.UniversalAdditionalCameraData>();
            lwrpCamData.renderShadows = true; // turn off shadows for the reflection camera
            lwrpCamData.requiresColorOption = UnityEngine.Rendering.Universal.CameraOverrideOption.Off;
            lwrpCamData.requiresDepthOption = UnityEngine.Rendering.Universal.CameraOverrideOption.Off;
            var reflectionCamera = go.GetComponent<Camera>();
            reflectionCamera.transform.SetPositionAndRotation(transform.position, transform.rotation);
            reflectionCamera.allowMSAA = currentCamera.allowMSAA;
            reflectionCamera.depth = -10;
            reflectionCamera.enabled = false;
            reflectionCamera.allowHDR = currentCamera.allowHDR;
            go.hideFlags = HideFlags.HideAndDontSave;

            return reflectionCamera;
        }


        public void ExecutePlanarReflections(ScriptableRenderContext context, Camera camera)
        {
            if (camera.cameraType == CameraType.Reflection)
                return;

#if UNITY_EDITOR
            if (SceneView.sceneViews.Count > 0)
            {
                SceneView sv = SceneView.sceneViews[0] as SceneView;

                if (sv != null && !sv.maximized && sv.in2DMode)
                    return;

                if (sv != null && sv.rotation.eulerAngles.x == 0 && sv.rotation.eulerAngles.z == 0)
                    return;
            }
#endif

            GL.invertCulling = true;
            //RenderSettings.fog = false;
            var max = QualitySettings.maximumLODLevel;
            var bias = QualitySettings.lodBias;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.lodBias = bias * 2f;

            UpdateReflectionCamera(camera);
            m_ReflectionCamera.cameraType = camera.cameraType;

            if (m_ReflectionTexture == null)
                CreateTextureFor(camera);

#if UNITY_EDITOR
            Vector2Int rtRes = RenderTextureResolution(camera);

            if (m_ReflectionCamera.targetTexture != null && (m_ReflectionTexture.width != rtRes.x || m_ReflectionTexture.height != rtRes.y))
            {
                m_ReflectionCamera.targetTexture = null;

                if (m_ReflectionTexture)
                {
                    RenderTexture.ReleaseTemporary(m_ReflectionTexture);
                    CreateTextureFor(camera);
                    m_ReflectionCamera.targetTexture = m_ReflectionTexture;
                }
            }
#endif    

            m_ReflectionCamera.targetTexture = m_ReflectionTexture;

            UnityEngine.Rendering.Universal.UniversalRenderPipeline.RenderSingleCamera(context, m_ReflectionCamera);

            GL.invertCulling = false;
            //RenderSettings.fog = true;
            QualitySettings.maximumLODLevel = 0;
            QualitySettings.lodBias = 8;

            if(m_SharedMaterial != null)
                m_SharedMaterial.SetTexture(reflectionSampler, m_ReflectionCamera.targetTexture);
        }

        void CreateTextureFor(Camera cam)
        {
            //Debug.Log("RT Created");
            Vector2Int rtRes = RenderTextureResolution(cam);

            bool useHDR10 = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.RGB111110Float);
            RenderTextureFormat hdrFormat = useHDR10 ? RenderTextureFormat.RGB111110Float : RenderTextureFormat.DefaultHDR;
            m_ReflectionTexture = RenderTexture.GetTemporary(rtRes.x, rtRes.y, 16, GraphicsFormatUtility.GetGraphicsFormat(hdrFormat, true));
        }

        Vector2Int RenderTextureResolution(Camera cam)
        {
            Vector2Int cameraSize;

            if (cam.cameraType == CameraType.SceneView)
                cameraSize = new Vector2Int(Camera.current.pixelWidth, Camera.current.pixelHeight);
            else
                cameraSize = new Vector2Int(Camera.main.pixelWidth, Camera.main.pixelHeight);

            int rtWidth = Mathf.FloorToInt(cameraSize.x / (1 + 10 - reflectionQuality));
            int rtHeight = Mathf.FloorToInt(cameraSize.y / (1 + 10 - reflectionQuality));

            return new Vector2Int(rtWidth, rtHeight);
        }

        static Matrix4x4 CalculateObliqueMatrix(Matrix4x4 projection, Vector4 clipPlane)
        {
            Vector4 q = projection.inverse * new Vector4(
                Sgn(clipPlane.x),
                Sgn(clipPlane.y),
                1.0F,
                1.0F
            );
            Vector4 c = clipPlane * (2.0F / (Vector4.Dot(clipPlane, q)));
            // third row = clip plane - fourth row
            projection[2] = c.x - projection[3];
            projection[6] = c.y - projection[7];
            projection[10] = c.z - projection[11];
            projection[14] = c.w - projection[15];

            return projection;
        }

        static float Sgn(float a)
        {
            if (a > 0.0F)
            {
                return 1.0F;
            }
            if (a < 0.0F)
            {
                return -1.0F;
            }
            return 0.0F;
        }

        // Calculates reflection matrix around the given plane
        private static void CalculateReflectionMatrix(ref Matrix4x4 reflectionMat, Vector4 plane)
        {
            reflectionMat.m00 = (1.0F - 2.0F * plane[0] * plane[0]);
            reflectionMat.m01 = (-2.0F * plane[0] * plane[1]);
            reflectionMat.m02 = (-2.0F * plane[0] * plane[2]);
            reflectionMat.m03 = (-2.0F * plane[3] * plane[0]);

            reflectionMat.m10 = (-2.0F * plane[1] * plane[0]);
            reflectionMat.m11 = (1.0F - 2.0F * plane[1] * plane[1]);
            reflectionMat.m12 = (-2.0F * plane[1] * plane[2]);
            reflectionMat.m13 = (-2.0F * plane[3] * plane[1]);

            reflectionMat.m20 = (-2.0F * plane[2] * plane[0]);
            reflectionMat.m21 = (-2.0F * plane[2] * plane[1]);
            reflectionMat.m22 = (1.0F - 2.0F * plane[2] * plane[2]);
            reflectionMat.m23 = (-2.0F * plane[3] * plane[2]);

            reflectionMat.m30 = 0.0F;
            reflectionMat.m31 = 0.0F;
            reflectionMat.m32 = 0.0F;
            reflectionMat.m33 = 1.0F;
        }

        private static Vector3 ReflectPosition(Vector3 pos)
        {
            Vector3 newPos = new Vector3(pos.x, -pos.y, pos.z);
            return newPos;
        }
    }
}
