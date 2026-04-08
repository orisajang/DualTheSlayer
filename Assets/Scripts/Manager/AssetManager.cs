using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AssetManager : MonoBehaviour
{
    Dictionary<string, Sprite> _imageSearchDic = new Dictionary<string, Sprite>();

    public IReadOnlyDictionary<string, Sprite> ImageSearchDic => _imageSearchDic;
    private void Awake()
    {
        Sprite[] spriteArray =  Resources.LoadAll<Sprite>("CardImage");
        foreach(Sprite image in spriteArray)
        {
            _imageSearchDic.Add(image.name, image);
        }
    }
    private void OnEnable()
    {
        GameManager.Instance.SetAssetManager(this);
    }
    private void OnDisable()
    {
        if (GameManager.isHaveInstance) GameManager.Instance.DeleteAssetManager(this);
    }
}
