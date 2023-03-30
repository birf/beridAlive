using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class DamagePopup : MonoBehaviour
{

    public static DamagePopup Create(Vector3 position, int damageAmount)
    {
        int absolute = damageAmount > 0 ? damageAmount : 0;
        
        Transform damagePopUpTransform = Instantiate(GameAssets.i.pfDamagePopup, position, Quaternion.identity);

        DamagePopup damagePopup = damagePopUpTransform.GetComponent<DamagePopup>();
        damagePopup.Setup(absolute);


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
        string txt = "";
        if (damageAmount > 0)
            txt = damageAmount.ToString();
        textMesh.SetText(txt);
        
        
        textColor = textMesh.color;
        dissapearTimer = 1f;
        if (damageAmount <= 0)
            transform.localScale = transform.localScale * 0.4f;
        moveVector = new Vector3(Random.Range(-0.25f,0.25f), 1) * 20f;
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
            if (textColor.a < 0 || transform.localScale.x < 0)
            {
                Destroy(gameObject);
            }
        }
    }
}
