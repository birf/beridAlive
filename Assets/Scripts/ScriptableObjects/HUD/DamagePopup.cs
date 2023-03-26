using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        Transform damagePopUpTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);


        DamagePopup damagePopup = damagePopUpTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(damageAmount);


        return damagePopup;
    }
    private TextMeshPro textMesh;
    private float dissapearTimer;
    private Color textColor;
    private Vector3 moveVector;

    private const float DISSAPEAR_TIMER_MAX = 1f;

    private void Awake()
    {
        textMesh = transform.GetComponent<TextMeshPro>();
    }
    public void Setup(int damageAmount)
    {
        textMesh.SetText(damageAmount.ToString());
        textColor = textMesh.color;
        dissapearTimer = 1f;

        moveVector = new Vector3(0, 1) * 20f;
    }
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        transform.position += moveVector * Time.deltaTime;
        moveVector -= moveVector * 8f * Time.deltaTime;



        if (dissapearTimer > DISSAPEAR_TIMER_MAX * .5f)
        {
            float increaseScaleAmount = 1f;

            transform.localScale += Vector3.one * increaseScaleAmount * Time.deltaTime;
        }
        else
        {
            float decreaseScaleAmount = 1f;

            transform.localScale -= Vector3.one * decreaseScaleAmount * Time.deltaTime;
        }

        dissapearTimer -= Time.deltaTime;
        if (dissapearTimer < 0)
        {
            float dissapearSpeed = 1.5f;
            textColor.a -= dissapearSpeed * Time.deltaTime;
            textMesh.color = textColor;
            if (textColor.a < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
