using UnityEngine;
using UnityEngine.UI;

public class TabMenuButton : Button
{
    public int index;

    public Sprite activeSprite;
    public Sprite inactiveSprite;

    public void SetAsActiveTabButton(bool active)
    {
        image.sprite = active ? activeSprite : inactiveSprite;
    }
}