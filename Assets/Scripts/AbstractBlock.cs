using UnityEngine;

public abstract class AbstractBlock : MonoBehaviour
{

    protected GameManager gameManager;
    void Start()
    {
        this.gameManager = GameObject.FindObjectOfType<GameManager>();
    }
    public abstract void AnimateRotation(Int2 center, string direction, float time);
    /// <summary>
    /// Will the block rotate?
    /// </summary>
    /// <returns>True if it will. DuH!!!!!!!</returns>
    public abstract bool isRotable();
    public abstract bool invalidatesRotation();

}