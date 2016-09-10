using UnityEngine;
using System.Collections;

public class ItemRender : MonoBehaviour {

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

        renderCamera.enabled = true;
        renderCamera.aspect = 1f;

        GameObject model = item.GetModel ();
        if (model) {

            model.transform.position = cur.transform.position;
            model.transform.parent = cur.transform;

            RenderTexture renderTexture = new RenderTexture (renderSize, renderSize, 24);
            renderTexture.Create ();

            Bounds bounds = GetObjectBounds (model);

            float camSize = Mathf.Max (Mathf.Abs (bounds.extents.y), Mathf.Abs (bounds.extents.z));
            renderCamera.orthographicSize = camSize;

            renderCamera.targetTexture = renderTexture;
            renderCamera.transform.position = cur.transform.position + Vector3.right - (cur.transform.position - bounds.center);
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

            model.SetActive (false);
            Destroy (model);
            return texture;
        }

        renderCamera.enabled = false;
        cur.gameObject.SetActive (false);

        return Resources.Load<Texture2D> ("GUI/PlaceholderImage");
    }

    public static Bounds GetObjectBounds (GameObject obj) {
        MeshFilter[] filters = obj.GetComponentsInChildren<MeshFilter> ();

        CombineInstance[] instances = new CombineInstance[filters.Length];
        for (int i = 0; i < filters.Length; i++) {
            instances[i].mesh = filters[i].sharedMesh;
            instances[i].transform = filters[i].transform.localToWorldMatrix;
        }

        Mesh newMesh = new Mesh ();
        newMesh.CombineMeshes (instances);
        newMesh.RecalculateBounds ();

        return newMesh.bounds;
    }
}
