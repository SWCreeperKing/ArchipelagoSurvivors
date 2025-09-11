using UnityEngine;

namespace ArchipelagoSurvivors;

public static class Helper
{
    public static GameObject[] GetChildren(this GameObject gobj)
    {
        var transform = gobj.transform;
        var count = transform.childCount;
        var children = new GameObject[count];

        for (var i = 0; i < count; i++)
        {
            children[i] = transform.GetChild(i).gameObject;
        }

        return children;
    }

    public static GameObject GetParent(this GameObject gobj) => gobj.transform.parent.gameObject;

    public static GameObject GetChild(this GameObject gobj, int index) => gobj.GetChildren()[index];

    public static GameObject[] GetChildren<TMonoBehavior>(this TMonoBehavior behavior)
        where TMonoBehavior : MonoBehaviour
        => behavior.gameObject.GetChildren();

    public static GameObject GetChild<TMonoBehavior>(this TMonoBehavior behavior, int index)
        where TMonoBehavior : MonoBehaviour
        => behavior.gameObject.GetChild(index);

    public static GameObject GetParent<TMonoBehavior>(this TMonoBehavior behavior) where TMonoBehavior : MonoBehaviour
        => behavior.transform.parent.gameObject;
}