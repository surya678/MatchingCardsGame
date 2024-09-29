using System.Collections.Generic;
using System.Collections;

using UnityEngine.UI;
using UnityEngine;
using TMPro;


public class GameManager : MonoBehaviour
{
    public GameObject cardPrefab; // Assign the card prefab in the inspector
    public GameObject cardsHolder,GameOverPanel, GameStartPanel;
    public GameObject VolumOffObj;
    public GameObject[] Stars;
    public TextMeshProUGUI RountCountText, MatchesCountText, BestRoundText;
    public Sprite[] Images;
    public Sprite CardBGImage;
    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    int rows = 4;
    int columns = 4;
    int bestScore;
    private bool isChecking = false; 
    int maxCombinations, rounds, Matches;    
    private int levelNum;
    int volumCheck = 0;
    public AudioClip CorrecClip, WrongClip, OverClip, clickClip;
    public AudioSource BGMSource, SoundSource;

    void Start()
    {
        GameStartPanel.SetActive(true);

        foreach (var star in Stars) star.SetActive(false);

        volumCheck = PlayerPrefs.GetInt("VolumeOn") == 1 ? 1 : 0;
        VolumeSettings();
    }
    public void SelectLevel(int level)
    {
        rows = columns = level;
        levelNum = level;
        Reset();
        PlaySound(clickClip);
    }

    private void SelectGrid()
    {
        var gridLayout = cardsHolder.GetComponent<GridLayoutGroup>();
        gridLayout.constraintCount = rows;
        gridLayout.cellSize = new Vector2(400 / rows, 400 / rows);
    }
   
    void SwapSprites()
    {
        // Shuffle the array
        for (int i = 0; i < Images.Length; i++)
        {
            int randomIndex = Random.Range(0, Images.Length);
            // Swap
            Sprite temp = Images[i];
            Images[i] = Images[randomIndex];
            Images[randomIndex] = temp;
        }       
    }

    void GenerateCards()
    {
        // Create card instances and position them in a grid
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < columns; j++)
            {
                GameObject cardObject = Instantiate(cardPrefab, new Vector3(j, -i, 0), Quaternion.identity);
                cardObject.transform.SetParent(cardsHolder.transform);
                cardObject.transform.localScale = Vector3.one;
                Card card = cardObject.GetComponent<Card>();
                card.backSprite = CardBGImage;
                cards.Add(card);
                // Set the card sprite based on your matching logic here
            }
        }

        for (int k = 0; k < maxCombinations; k++)
        {
            cards[k].frontSprite = Images[k];
            cards[maxCombinations + k].frontSprite = Images[k];
        }
    }

    void SwapButtons()
    {
        // Shuffle the list of buttons
        for (int i = 0; i < cards.Count; i++)
        {
            int randomIndex = Random.Range(0, cards.Count);
            // Swap the buttons in the list
            Card temp = cards[i];
            cards[i] = cards[randomIndex];
            cards[randomIndex] = temp;
        }

        // Rearrange the buttons in the GridLayoutGroup
        for (int i = 0; i < cards.Count; i++)
        {
            cards[i].transform.SetSiblingIndex(i);
        }
    }

    public void OnCardSelect(Card clickedCard)
    {
        PlaySound(clickClip);

        if (isChecking || clickedCard.IsFaceUp())
            return;

        clickedCard.Flip();

        if (firstCard == null)
        {
            firstCard = clickedCard;
        }
        else
        {
            secondCard = clickedCard;
            isChecking = true;
            StartCoroutine(MatchCheck());
        }
    }
    public void Reset()
    {
        foreach (Transform child in cardsHolder.transform)
        {
            Destroy(child.gameObject);
        }
        cards.Clear();

        foreach (var star in Stars) star.SetActive(false);

        rounds = 0;
        Matches = 0;
        MatchesCountText.text = Matches.ToString();
        RountCountText.text = rounds.ToString();
        GameStartPanel.SetActive(false);
        SwapSprites();
        GameOverPanel.SetActive(false);
        maxCombinations = (rows * columns) / 2;

        SelectGrid();

        GenerateCards();
        SwapButtons();

        BestRoundText.text = DisplayBestScore();
    }

  
    private IEnumerator MatchCheck()
    {
        rounds++;
        yield return new WaitForSeconds(1f); // Wait for flipping animation to finish

        if (firstCard.frontSprite == secondCard.frontSprite) // Check if matched
        {
            Matches++;
            PlaySound(CorrecClip);

            if (Matches == maxCombinations)
            {
                yield return new WaitForSeconds(1f);
                GameOver();
            }

        }
        else
        {
            PlaySound(WrongClip);
            firstCard.Flip(); // Flip back
            secondCard.Flip();
        }
        UpdateScore();
        firstCard = null;
        secondCard = null;
        isChecking = false;

        Debug.Log("rounds : " + rounds + " Matches :" + Matches);
    }
    private void UpdateScore()
    {
        RountCountText.text = rounds.ToString();
        MatchesCountText.text = Matches.ToString();
    }
    public void GameOver()
    {
        PlaySound(OverClip);
        GameOverPanel.SetActive(true);
        DisplayStars();
        UpdateBestScore();
        BestRoundText.text = DisplayBestScore();

    }

    private string DisplayBestScore()
    {
        return Getint(levelNum == 2 ? "easyLevel" : levelNum == 4 ? "MediumLevel" : "HardLevel").ToString();
    }  
   
    private void DisplayStars()
    {
        foreach (var star in Stars) star.SetActive(false);
        if (Matches == rounds)
        {
            foreach (var star in Stars) star.SetActive(true);
        }
        else if (rounds <= Matches * 2)
        {
            Stars[0].SetActive(true);
            Stars[1].SetActive(true);
        }
        else
        {
            Stars[0].SetActive(true);
        }
    }

    private void UpdateBestScore()
    {
        int bestScore = Getint(levelNum == 2 ? "easyLevel" : levelNum == 4 ? "MediumLevel" : "HardLevel");

        if (bestScore == 0 || rounds < bestScore)
        {
            SetInt(levelNum == 2 ? "easyLevel" : levelNum == 4 ? "MediumLevel" : "HardLevel", rounds);
        }
    }
    public void Quit()
    {
        PlaySound(clickClip);
        Application.Quit();
    }

    public void Restart()
    {
        PlaySound(clickClip);
        UnityEngine.SceneManagement.SceneManager.LoadScene(0);
    }

    public void SetInt(string KeyName, int Value)
    {
        PlayerPrefs.SetInt(KeyName, Value);
    }

    public int Getint(string KeyName)
    {
        return PlayerPrefs.GetInt(KeyName);
    }

    public void PlaySound(AudioClip clip)
    {
        SoundSource.clip = clip;
        SoundSource.Play();
    }

    public void VolumeOff()
    {
        volumCheck ^= 1; // Toggle volume
        VolumeSettings();
        PlayerPrefs.SetInt("VolumeOn", volumCheck);
    }

    private void VolumeSettings()
    {
        VolumOffObj.SetActive(volumCheck == 1);
        BGMSource.mute = volumCheck == 1;
        SoundSource.mute = volumCheck == 1;
    }
}
