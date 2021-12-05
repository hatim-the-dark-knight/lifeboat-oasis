using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CharacterCustomizer : MonoBehaviour
{
    public GameObject[] upperwear, lowerwear, headwear;
    public int current_upperwear, current_lowerwear, current_headwear;
    public PlayerNetworkManager PNManager;
    public ColorPicker skinColor;//
    public ColorPicker topColor;
    public ColorPicker bottomColor;
    // Start is called before the first frame update
    void Start()
    {
        Color skin;
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(PlayerPrefs.GetString("SkinColor"),"FFFFFF"), out skin);

        Color top;
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(PlayerPrefs.GetString("TopColor"),"FFFFFF"), out top);

        Color bottom;
        ColorUtility.TryParseHtmlString("#" + PlayerPrefs.GetString(PlayerPrefs.GetString("BottomColor"),"FFFFFF"), out bottom);

        // skinColor.Create(skin,null,null,null);
        // topColor.Create(top,null,null,null);
        // bottomColor.Create(bottom,null,null,null);
    }

    // Update is called once per frame
    void Update()
    {
        // for (int i = 0; i < upperwear.Length; i++)
        // {
        //     if(i == current_upperwear)
        //         upperwear[i].SetActive(true);
        //     else
        //         upperwear[i].SetActive(false);
        // }
        // for (int i = 0; i < lowerwear.Length; i++)
        // {
        //     if(i == current_lowerwear)
        //         lowerwear[i].SetActive(true);
        //     else
        //         lowerwear[i].SetActive(false);
        // }
        // for (int i = 0; i < headwear.Length; i++)
        // {
        //     if(i == current_headwear)
        //         headwear[i].SetActive(true);
        //     else
        //         headwear[i].SetActive(false);
        // }
    }

    public void SetSkinColor()
    {
        PlayerPrefs.SetString("SkinColor",ColorUtility.ToHtmlStringRGB(skinColor.colorComponent.color));//
        PNManager.AssignPlayerCustomisationData();
    }
    public void SetTopColor()
    {
        PlayerPrefs.SetString("TopColor",ColorUtility.ToHtmlStringRGB(topColor.colorComponent.color));//
        PNManager.AssignPlayerCustomisationData();
    }

    public void SetBottomColor()
    {
        PlayerPrefs.SetString("BottomColor",ColorUtility.ToHtmlStringRGB(bottomColor.colorComponent.color));//
        PNManager.AssignPlayerCustomisationData();
    }
    public void SetUpperWear(int index)
    {
        PlayerPrefs.SetInt("UpperWear",index);
        current_upperwear = index;
        PNManager.AssignPlayerCustomisationData();
    }

    public void SetLowerWear(int index)
    {
        PlayerPrefs.SetInt("LowerWear",index);
        current_lowerwear = index;
        PNManager.AssignPlayerCustomisationData();
    }

    public void SetHeadWear(int index)
    {
        PlayerPrefs.SetInt("HeadWear",index);
        current_headwear = index;
        PNManager.AssignPlayerCustomisationData();
    }

    public void MainMenu()
    {
        SceneManager.LoadScene(0);
    }
}
