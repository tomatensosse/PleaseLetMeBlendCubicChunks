using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class CategoryUI : MonoBehaviour
{
    public TMP_Text categoryNameText;
    public List<Image> categoryIconImages;
    public Transform itemContainer;

    public void Initialize(ItemCategory category)
    {
        foreach (Image iconImage in categoryIconImages)
        {
            iconImage.sprite = category.categoryIcon;
        }

        categoryNameText.text = category.categoryDisplayName;
    }
}
