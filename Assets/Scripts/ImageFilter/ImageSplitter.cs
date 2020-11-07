using UnityEngine;
using System.Collections;

namespace ImageSplitter {

    public class ImageSplitter : MonoBehaviour
    {
        public Texture2D tex;  // Texture to cut up
        public Material mat;   // Material to use (texture is discarded)
        public int cols = 6;   // Number of tiles across
        public float aspect = 1.5f;  // Original aspect (height / width)

        void Start()
        {
            mat.mainTexture = tex;
            BuildPieces();
        }

        void BuildPieces()
        {
            int rows = Mathf.RoundToInt(cols * aspect);
            Vector3 offset = Vector3.zero;
            offset.x = -Mathf.RoundToInt((float)cols / 2.0f - 0.5f);
            offset.y = -Mathf.RoundToInt((float)rows / 2.0f - 0.5f);
            float startX = offset.x;
            float uvWidth = 1.0f / cols;
            float uvHeight = 1.0f / rows;


            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    GameObject go = GameObject.CreatePrimitive(PrimitiveType.Quad);
                    Transform t = go.transform;
                    t.position = offset;
                    t.localScale = new Vector3(0.95f, 0.95f, 0.95f);
                    go.GetComponent<Renderer>().material = mat;
                    //go.renderer.material = mat;

                    Mesh mesh = go.GetComponent<MeshFilter>().mesh;
                    Vector2[] uvs = mesh.uv;
                    uvs[0] = new Vector2(j * uvWidth, i * uvHeight);
                    uvs[3] = new Vector2(j * uvWidth, (i + 1) * uvHeight);
                    uvs[1] = new Vector2((j + 1) * uvWidth, (i + 1) * uvHeight);
                    uvs[2] = new Vector2((j + 1) * uvWidth, i * uvHeight);
                    mesh.uv = uvs;
                    offset.x += 1.0f;
                }
                offset.y += 1.0f;
                offset.x = startX;
            }
        }
    }

}
