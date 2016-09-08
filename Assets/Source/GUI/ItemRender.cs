using UnityEngine;
using System.Collections;

public class ItemRender : MonoBehaviour {

    public MeshFilter filter;
    public Camera renderCamera;
    public static int renderSize = 100;

    public static ItemRender cur;

    void Awake () {
        cur = this;
        cur.gameObject.SetActive (false);
    }

	public static Texture2D RenderItem (Item item) {
        cur.gameObject.SetActive (true);

        Camera renderCamera = cur.renderCamera;
        MeshFilter meshFilter = cur.filter;

        renderCamera.enabled = true;
        renderCamera.aspect = 1f;

        Mesh mesh = item.GetMesh ();
        if (mesh) {
            meshFilter.mesh = mesh;

            RenderTexture renderTexture = new RenderTexture (renderSize, renderSize, 24);
            renderTexture.Create ();

            Vector3 bounds = mesh.bounds.extents;

            float camSize = Mathf.Max (Mathf.Abs (bounds.y), Mathf.Abs (bounds.z));
            renderCamera.orthographicSize = camSize;

            renderCamera.targetTexture = renderTexture;
            renderCamera.transform.position = cur.transform.position + Vector3.right - (cur.transform.position - meshFilter.GetComponent<Renderer> ().bounds.center);
            renderCamera.Render ();

            RenderTexture.active = renderTexture;

            Texture2D texture = new Texture2D (renderSize, renderSize, TextureFormat.ARGB32, false);
            texture.ReadPixels (new Rect (0f, 0f, renderSize, renderSize), 0, 0);
            texture.Apply ();

            renderCamera.targetTexture = null;
            RenderTexture.active = null;

            Destroy (renderTexture);

            renderCamera.enabled = false;
            cur.gameObject.SetActive (false);

            return texture;
        }

        renderCamera.enabled = false;
        cur.gameObject.SetActive (false);

        return Resources.Load<Texture2D> ("GUI/PlaceholderImage");
    }
}
