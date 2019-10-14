using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class NodeEditor : EditorWindow
{
    public const int CELLSIZE = 75;
    public const int SPACING = 50;
    Rect window1;
    Rect window2;

    List<DialogPiece> renderedPieces = new List<DialogPiece>();

    [MenuItem("Window/Node editor")]
    static void ShowEditor()
    {
        NodeEditor editor = EditorWindow.GetWindow<NodeEditor>();
        editor.Init();
    }

    public void Init()
    {
        Rect firstWIndow = new Rect(10, 10, CELLSIZE, CELLSIZE);
    }

    void OnGUI()
    {
        if(Selection.activeObject == null)
        {
            return;
        }

        if(Selection.activeObject.GetType() != typeof(DialogPiece))
        {
            return;
        }
        DialogPiece piece = (DialogPiece) Selection.activeObject;


        BeginWindows();

        renderedPieces.Clear();
        DrawDialog(piece,1, SPACING, SPACING);

        EndWindows();
    }

    void DrawDialog(DialogPiece piece,int id, int x, int y)
    {
        if (renderedPieces.Contains(piece))
        {
            return;
        }
        renderedPieces.Add(piece);

        Rect windowRCT = new Rect(x, y, CELLSIZE, CELLSIZE);
        GUIStyle style = new GUIStyle();
        style.alignment = TextAnchor.MiddleCenter;
        style.imagePosition = ImagePosition.ImageAbove;
        GUI.Box(windowRCT, new GUIContent(piece.name,piece.Image.texture), style);

        foreach (DialogDecision decision in piece.Decisions)
        {
            Rect decisionRect = new Rect(x+CELLSIZE+ SPACING, y + piece.Decisions.IndexOf(decision)*(CELLSIZE + SPACING), CELLSIZE, CELLSIZE);
            GUI.Box(decisionRect, new GUIContent(decision.name, decision.Icon.texture), style);
            DrawNodeCurve(windowRCT, decisionRect);

            if(decision.NextPiece != null)
            {
                DrawDialog(decision.NextPiece, id + piece.Decisions.Count+1, x + piece.Decisions.IndexOf(decision) * (CELLSIZE * 2 + SPACING * 2), y+ piece.Decisions.IndexOf(decision) * (CELLSIZE * 2 + SPACING * 2));
            }
        }
    }

    void DrawNodeWindow(int ttid)
    {
        GUI.DragWindow();
    }

    void DrawNodeCurve(Rect start, Rect end)
    {
        Vector3 startPos = new Vector3(start.x + start.width, start.y + start.height / 2, 0);
        Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
        Vector3 startTan = startPos + Vector3.right * 50;
        Vector3 endTan = endPos + Vector3.left * 50;
        Color shadowCol = new Color(0, 0, 0, 0.06f);
        for (int i = 0; i < 3; i++) // Draw a shadow
            Handles.DrawBezier(startPos, endPos, startTan, endTan, shadowCol, null, (i + 1) * 5);
        Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
    }
}