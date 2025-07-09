using UnityEngine;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardID;
    public Sprite frontSprite;
    public Sprite backSprite;
    public Image cardImage;

    private MemoryGameManager gameManager;
    private bool isFlipped = false;

    public void Setup(int id, Sprite front, MemoryGameManager manager)
    {
        cardID = id;
        frontSprite = front;
        gameManager = manager;

        cardImage.sprite = backSprite;
        transform.rotation = Quaternion.Euler(0, 0, 0);
        isFlipped = false;
    }

    public void OnClickCard()
    {
        if (!isFlipped && gameManager.CanFlip())
        {
            gameManager.CardRevealed(this);
            FlipCard(true);
        }
    }

    public void FlipCard(bool showFront)
    {
        isFlipped = showFront;

        LeanTween.rotateY(gameObject, 90f, 0.2f).setOnComplete(() =>
        {
            cardImage.sprite = showFront ? frontSprite : backSprite;
            LeanTween.rotateY(gameObject, showFront ? 180f : 0f, 0.2f);
        });
    }

    public void FlipBack()
    {
        if (isFlipped)
            FlipCard(false);
    }
}
