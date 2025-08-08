using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ItemUI : MonoBehaviour
{
    public Image itemIcon;
    public TMP_Text itemNameText;
    public TMP_Text itemAmountText;
    public Button itemButton;

    public int Index => index;

    private ItemInstance itemInstance;
    private int index;

    public void Set(ItemInstance itemInstance, int index)
    {
        this.itemInstance = itemInstance;
        this.index = index;

        itemIcon.sprite = itemInstance.Item.icon;
        itemNameText.text = itemInstance.Item.name;
        itemAmountText.text = itemInstance.amount > 1 ? itemInstance.amount.ToString() : string.Empty;

        //itemButton.onClick.AddListener(OnItemClicked);
    }
}
