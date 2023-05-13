using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BombScript : MonoBehaviour
{
    [SerializeField] GameObject explosionVFX;
    [SerializeField] float explosionDelay= 3f, vfxDuration = 5f;
    [SerializeField] float blastRadius=3;
    [SerializeField] int blastPower =3;
    [SerializeField] float bombRadius=1;

    bool reflectedByPlayer = false;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(ExplosionCoroutine(explosionDelay));
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    IEnumerator ExplosionCoroutine(float delay=1f)
    {
        yield return new WaitForSeconds(delay);
        Explode();
    }

    private void Explode()
    {
        if(GameManager.Instance.currentState!= GameManager.GameStates.Active) return;
        Destroy(Instantiate(explosionVFX, transform.position, explosionVFX.transform.rotation),vfxDuration);
        Collider[] colliders = Physics.OverlapSphere(transform.position, blastRadius);
        foreach(Collider collider in colliders)
        {
            if(collider.TryGetComponent<Health>(out Health healthComp))
            {
                // damage for ship
                if(reflectedByPlayer && collider.gameObject.CompareTag("enemy"))
                {
                    healthComp.OnTakeDamage(1);
                    continue;
                } 
                // damage for planks
                float dis = Vector3.Distance(transform.position, collider.gameObject.transform.position)-bombRadius*2;
                float disRate = dis/(blastRadius);
                disRate = Mathf.Clamp01(disRate);
                float damage  = blastPower - blastPower*disRate;
                healthComp.OnTakeDamage(Mathf.FloorToInt(damage));
            }
            
        }
        Destroy(gameObject);
    }
}
