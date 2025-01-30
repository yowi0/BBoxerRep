using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public GameObject ui;
    public GameObject cubeManager;
    public Dropdown musicDropDown;
    public AudioSource gameMusic;
    public AudioClip[] audioClips;
    
    void Start()
    {
        ui.SetActive(true);
        cubeManager.SetActive(false);
        
        if (musicDropDown != null)
        {
            musicDropDown.onValueChanged.AddListener(HandleDropdownChange);
        }
    }
    
    public void StartGame()
    {
        ui.SetActive(false);
        cubeManager.SetActive(true);
    }
    
    void HandleDropdownChange(int index)
    {
        if (audioClips != null && index >= 0 && index < audioClips.Length)
        {
            gameMusic.clip = audioClips[index];
            gameMusic.Play();
        }
    }
}
