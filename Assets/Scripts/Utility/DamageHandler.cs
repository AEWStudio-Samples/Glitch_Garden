using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    // con fig vars //
    public GUIControll guiCon = null;
    public GameObject damageText = null;
    public GameObject critDamageText = null;
    public GameObject hostileDamageText = null;

    public void DealDamage(int damage, Collider2D target)
    {
        if (!target) { return; }

        switch (target.tag)
        {
            case "Ninja":
                target.GetComponent<NinjaControll>().HandleHit(damage);
                break;
            case "Wall":
                target.GetComponent<WallControll>().HandleHit(damage);
                break;
            case "Zombie":
                target.GetComponent<ZombieControll>().HandleHit(damage);
                break;
            case "Phantom":
                target.GetComponent<PhantomControll>().HandleHit(damage);
                break;
        }
    }

    public bool CheckCrit(int cr)
    {
        if (Random.Range(1, 101) < cr) { return true; }

        return false;
    }

    public void SpawnDMGText(int damage, Vector3 spawPoint)
    {
        GameObject dmgText = Instantiate(damageText,
            spawPoint, Quaternion.identity);
        dmgText.GetComponent<DmgText>().SetText(damage);
    }

    public void SpawnCDMGText(int damage, Vector3 spawPoint)
    {
        GameObject dmgText = Instantiate(critDamageText,
            spawPoint, Quaternion.identity);
        dmgText.GetComponent<DmgText>().SetText(damage);
    }

    public void SpawnHDMGText(int damage, Vector3 spawPoint)
    {
        GameObject dmgText = Instantiate(hostileDamageText,
            spawPoint, Quaternion.identity);
        dmgText.GetComponent<DmgText>().SetText(damage);
    }
}
