using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MOFADuelThumbnail : MonoBehaviour
{

    [SerializeField] List<GameObject> _boltPrefab = new List<GameObject>();
    [SerializeField] List<Animator> _playerAnimator = new List<Animator>();

    // Start is called before the first frame update
    void Start()
    {
        _playerAnimator[0].SetFloat("Velocity X", 1);
        _playerAnimator[1].SetFloat("Velocity X", 1);

        StartCoroutine(WaitAndShootBolt());
    }

    // Update is called once per frame
    void Update()
    {
        
    }



    IEnumerator WaitAndShootBolt()
    {
        yield return new WaitForSeconds(1.25f);
        PlayAttackAnimation(0);
        yield return new WaitForSeconds(0.25f);
        ShootBolt(0);
        yield return new WaitForSeconds(1.25f);
        PlayAttackAnimation(1);
        yield return new WaitForSeconds(0.25f);
        ShootBolt(1);

        StartCoroutine(WaitAndShootBolt());
    }

    void PlayAttackAnimation(int index)
    {
        _playerAnimator[index].SetTrigger("Attack A");
    }

    void ShootBolt(int index)
    {
        var go = Instantiate(_boltPrefab[index]);
        var player = _playerAnimator[index].transform;
        go.transform.position = player.position + player.forward * 1f + Vector3.up * 1.5f;
        go.GetComponent<Rigidbody>().velocity = player.forward * 2f;
    }
}