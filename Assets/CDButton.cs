using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CDButton : MonoBehaviour
{
    [SerializeField] Transform item;
    public void CopyItem()
    {
        CopyItem(item.GetSiblingIndex() + 1);
    }
    public void CopyItem(Transform add)
    {
        CopyItem(add.GetSiblingIndex());
    }
    public GameObject CopyItem(int idx)
    {
        GameObject obj = Instantiate(item.gameObject, item.parent, false);
        obj.name.Replace("(Clone)", "");
        obj.GetComponent<Transform>().SetSiblingIndex(idx);
        obj.SetActive(true);
        active.Refresh(item);
        return obj;
    }
    public void DeleteItem()
    {
        item.gameObject.SetActive(false);
        active.Refresh(item);
        Destroy(item.gameObject);
    }

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
