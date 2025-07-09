using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MemoryGameManager : MonoBehaviour
{
    public GameObject cardPrefab;
    public Transform cardParent;
    public Sprite[] cardSprites;

    public TMP_Text flipText;
    public TMP_Text scoreText;
    public GameObject homePanel;
    public GameObject winPanel;
   

    private List<Card> cards = new List<Card>();
    private Card firstCard, secondCard;
    private bool isProcessing = false;

    private int flipCount = 0;
    private int score = 0;

    void Start()
    {
        ShowHome();
    }

    public void StartGame()
    {
        homePanel.SetActive(false);
        winPanel.SetActive(false);
        ClearCards();
        SetupCards();
        flipCount = 0;
        score = 0;
        UpdateUI();
    }

    public void ShowHome()
    {
        homePanel.SetActive(true);
        winPanel.SetActive(false);
        ClearCards();
    }

    public void ResetGame()
    {
        StartGame();
    }

    void SetupCards()
    {
        List<int> ids = new List<int>();
        for (int i = 0; i < cardSprites.Length; i++)
        {
            ids.Add(i);
            ids.Add(i);
        }

        Shuffle(ids);

        for (int i = 0; i < ids.Count; i++)
        {
            GameObject newCard = Instantiate(cardPrefab, cardParent);
            Card card = newCard.GetComponent<Card>();
            card.Setup(ids[i], cardSprites[ids[i]], this);
            newCard.GetComponent<Button>().onClick.AddListener(() => card.OnClickCard());
            cards.Add(card);
        }
    }

    void Shuffle(List<int> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            int rnd = Random.Range(i, list.Count);
            int temp = list[i];
            list[i] = list[rnd];
            list[rnd] = temp;
        }
    }

    public void CardRevealed(Card card)
    {
        if (!CanFlip()) return;

        flipCount++;
        UpdateUI();
        AudioManager.Instance.PlayFlip();

        if (firstCard == null)
        {
            firstCard = card;
        }
        else if (secondCard == null)
        {
            secondCard = card;
            StartCoroutine(CheckMatch());
        }
    }

    IEnumerator CheckMatch()
    {
        isProcessing = true;
        yield return new WaitForSeconds(1f);

        if (firstCard.cardID == secondCard.cardID)
        {
            score++;
            UpdateUI();
            AudioManager.Instance.PlayMatch();

            if (score == cardSprites.Length)
            {
                winPanel.SetActive(true);
                AudioManager.Instance.PlayWin();
            }
        }
        else
        {
            AudioManager.Instance.PlayMismatch();
            firstCard.FlipBack();
            secondCard.FlipBack();
        }

        firstCard = null;
        secondCard = null;
        isProcessing = false;
    }

    public bool CanFlip()
    {
        return !isProcessing && secondCard == null;
    }

    void UpdateUI()
    {
        flipText.text = "Flips: " + flipCount;
        scoreText.text = "Score: " + score;
    }

    void ClearCards()
    {
        foreach (var card in cards)
        {
            if (card != null)
                Destroy(card.gameObject);
        }

        cards.Clear();
        firstCard = null;
        secondCard = null;
    }

    public void Quit()
    {
        Application.Quit();
    }
}
