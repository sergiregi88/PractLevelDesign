using UnityEngine;

class FloatingText : MonoBehaviour
{
    private static readonly GUISkin Skin = Resources.Load<GUISkin>("GameSkin");    
    public static FloatingText Show(string text,string style, IFloatingTextPositioner posititoner )
    {
        Debug.Log("www");
        var go = new GameObject("FloatingText");
        var floatingText = go.AddComponent<FloatingText>();
        floatingText.Style = Skin.GetStyle(style);
        floatingText._positioner = posititoner;
        floatingText._content = new GUIContent(text);
        Debug.Log(floatingText);
        return floatingText;
    }
    private GUIContent _content;
    private IFloatingTextPositioner _positioner;
    
    public string Text { get { return _content.text; } set { _content.text = value; } }
    public GUIStyle Style { get; set; }

    public void OnGUI()
    {

        var position = new Vector2();
        var contentSize = Style.CalcSize(_content);
        if (!_positioner.GetPosition(ref position, _content, contentSize)) 
        {
            Destroy(gameObject);
            return;
        }
       
        GUI.Label(new Rect(position.x, position.y, contentSize.x, contentSize.y),_content,Style);

    }

}

