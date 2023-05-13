using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonScript : MonoBehaviour
{
    [SerializeField] private Vector2 shootInterval = new Vector2(1,2); 
    [SerializeField] private float rangeInDegrees = 15f; 
    [SerializeField] private float arcInDeegrees = 15f; 
    [SerializeField] List<GameObject> ammoPrefab;
    [SerializeField]Transform ammonSpawnPoint, target;
    [SerializeField] Vector2 shootingForce = new Vector2(100,100);
    [SerializeField] float  rotatioDuration= 1f;
    private float shootCooldown = 0;
    private float startingInclination;
    public bool isAiming = false;
    private Coroutine lookatRoutine;
    // Update is called once per frame
    void Update()
    {
        if(GameManager.Instance.currentState!= GameManager.GameStates.Active) return;
        if(isAiming) return;

        shootCooldown -= Time.deltaTime;
        if(shootCooldown<0)
        {
            StartCoroutine(ShootCoroutine());  
        }   
    }
    public void Shoot()
    {
        shootCooldown = Random.Range(shootInterval.x, shootInterval.y);
        Rigidbody ammoRigidbody = PrepareAmmo().GetComponent<Rigidbody>();
        ammoRigidbody.AddForce(PrepareShootVector(), ForceMode.Impulse);
    }

    private Vector3 PrepareShootVector()
    {
        Vector3 impulseVector = ammonSpawnPoint.forward;

        impulseVector.Scale(new Vector3(1, 0, 1));
        impulseVector.Normalize();
        impulseVector += new Vector3(0, arcInDeegrees / 45f, 0);

        impulseVector.Normalize();
        impulseVector = Quaternion.AngleAxis(rangeInDegrees * Random.Range(-1f, 1f), Vector3.up) * impulseVector;
        impulseVector *= Random.Range(shootingForce.x, shootingForce.y);
        return impulseVector;
    }
    private GameObject PrepareAmmo()
    {
        int ammonIndex = Random.Range(0, ammoPrefab.Count);
        GameObject ammo =
            Instantiate(ammoPrefab[ammonIndex],
            ammonSpawnPoint.position,
            ammoPrefab[ammonIndex].transform.rotation);
        return ammo;
    }

    public void LookAtTarget()
    {
        if(lookatRoutine !=null)
        {
            StopCoroutine(lookatRoutine);
        }
        lookatRoutine = StartCoroutine(LookAtCoroutine());
       

    }
    IEnumerator ShootCoroutine()
    {
        LookAtTarget();
        yield return new WaitUntil(()=>!isAiming);
        Shoot();
    }
    IEnumerator LookAtCoroutine()
    {
        if(isAiming)
        {
            yield break;
        }
        isAiming = true;
        Vector3 lookAt = new Vector3(target.position.x,0,target.position.z) -  new Vector3(transform.position.x,0,transform.position.z) ;
        Quaternion lookRotation = Quaternion.LookRotation(lookAt);
        

        float time = 0;
        while(time<rotatioDuration)
        {
            time += Time.deltaTime;
            transform.rotation  = Quaternion.Lerp(transform.rotation, lookRotation, time/rotatioDuration);            
            yield return null;
        }
        
        isAiming = false;   
    }
    
}
