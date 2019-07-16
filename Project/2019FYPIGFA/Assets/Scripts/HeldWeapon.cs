using System;
using UnityEngine;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
[RequireComponent(typeof(Collider))]
public class HeldWeapon : MonoBehaviour
{
    [HideInInspector]
    public ItemData itemData;
    public Player player;
    private float attackTimer = 0f;
    ProjectilePool projectilePoolInstance;
    private static int projectileID;
    // Start is called before the first frame update
    void Start()
    {
        attackTimer = 0f;
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
        transform.localPosition = itemData.heldPosition;
        transform.localRotation = Quaternion.Euler(itemData.heldRotation);
        player = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Player>();
        projectilePoolInstance = ProjectilePool.g_sharedInstance;
        projectileID = projectilePoolInstance.GetPooledObjectIndex(itemData.projectile);
        if (null == projectilePoolInstance)
            Debug.LogError("The projectile pool no exist!");

    }

    // Update is called once per frame
    void Update()
    {
        if (attackTimer > 0)
            attackTimer -= Time.deltaTime;
        else
            attackTimer = 0f;
    }

    public void ChangeWeapon(ItemData newWeap)
    {
        itemData = newWeap;
        GetComponent<MeshFilter>().mesh = itemData.mesh;
        GetComponent<MeshRenderer>().material = itemData.material;
        transform.localPosition = itemData.heldPosition;
        transform.localRotation = Quaternion.Euler(itemData.heldRotation);
    }

    public ItemData RemoveWeapon()
    {
        ItemData tempItem = itemData.Clone();
        itemData.weaponType = ItemData.WEAPON_TYPE.NONE;
        itemData.weaponDamage = 0;
        itemData.attackRate = 0;
        itemData.durability = 0;
        itemData.durabilityDecay = 0;
        itemData.attackRange = 0;
        itemData.type = null;
        itemData.mesh = null;
        itemData.material = null;
        itemData.heldPosition = Vector3.zero;
        itemData.heldRotation = Vector3.zero;
        itemData.impactEffect = null;
        itemData = null;
        Destroy(GetComponent<MeshFilter>().mesh);
        Destroy(GetComponent<MeshRenderer>().material);
        return tempItem;
    }

    public bool Fire()
    {
        RaycastHit hit;
        switch (itemData.weaponType)
        {
            case ItemData.WEAPON_TYPE.RAYCAST:
                // attackrate = times per second can attack
                // attacktimer = when zero, can attack
                // attack timer set to (1 / attackrate) when attack
                if (Physics.Raycast(Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)), out hit, itemData.attackRange))
                {
                    if (attackTimer > 0f)
                        return false;

                    if (hit.collider.GetComponent<Enemy>() != null) // TODO: APPLY DEBUFF HERE
                    {
                        Enemy enemy = hit.collider.GetComponent<Enemy>();
                        enemy.health = Mathf.Clamp(enemy.health - itemData.weaponDamage, 0, enemy.maxHealth);
                        attackTimer = 1 / itemData.attackRate;
                        itemData.durability = Mathf.Clamp(itemData.durability - itemData.durabilityDecay, 0, 100);
                        Debug.Log("Hit " + enemy.enemyType + " for " + itemData.weaponDamage + " damage with " + itemData.type + ". Durability Left: " + itemData.durability + "%");
                    }

                    if (itemData.impactEffect)
                    {
                        GameObject impackEfek = Instantiate(itemData.impactEffect, hit.point, Quaternion.LookRotation((transform.position - hit.point).normalized), hit.transform);
                        Destroy(impackEfek, 2f);
                    }
                }
                break;
            case ItemData.WEAPON_TYPE.PROJECTILE:
                // Spawn the projectile at where player shoots
                I_Projectile bullet = ProjectilePool.g_sharedInstance.FetchObjectInPool(itemData.projectileID).GetComponent<I_Projectile>();
                bullet.Initialize();
                bullet.Discharge(Camera.main.transform.forward * itemData.projectileMagnitude, transform.position + Camera.main.transform.forward * 1f);
                //// Uses the object pooler to spawn the bullet
                //int i = projectilePoolInstance.GetPooledObjectIndex(itemData.projectile);
                //I_Projectile projectile = projectilePoolInstance.FetchObjectInPool(i).GetComponent<I_Projectile>();
                //if (null == projectile)
                //{
                //    Debug.LogWarning("Did not get any projectile");
                //    break;
                //}
                //projectile.Discharge(player.yLookObject.transform.forward * 10f, player.yLookObject.transform.position /*+ player.yLookObject.transform.forward*/);
                break;
            default:
                Debug.LogError("Assign weapon type before trying to attack.");
                break;
        }


        return true;
    }
    public bool Skill()
    {
        switch (itemData.skillType)
        {
            case ItemData.SKILL_TYPE.LUNGE:
                player.AddExternalForce(Camera.main.transform.forward * 10f);
                // Now add a collider in front of the main camera
                GameObject attackCollider = projectilePoolInstance.FetchObjectInPool(itemData.projectileID);
                Debug.Log("Spawned object " + attackCollider);
                Vector3 scale = new Vector3(1f, 1f, 1f);
                attackCollider.GetComponent<I_Projectile>().Initialize();
                attackCollider.transform.localScale = scale;
                attackCollider.transform.position = Camera.main.transform.position + Camera.main.transform.forward * 0.3f;
                attackCollider.transform.rotation = Camera.main.transform.rotation;
                attackCollider.transform.parent = Camera.main.transform;
                break;
            case ItemData.SKILL_TYPE.MAYO_DRINK:
                //player.ApplyBuff(Buffable.CHAR_BUFF.BUFF_SLOMO, 5f);
                Debug.Log("Applied mayo drink");
                break;
            default:
                Debug.LogError("This skill is unheard of!");
                break;
        }

        return true;
    }
    public void ApplyBuff(ItemData.WeaponBuff _buff)
    {
        if (ItemData.BUFF_TYPE.NONE != itemData.weaponBuff.buff)
            Debug.Log("Just replaced buff of ID " + itemData.weaponBuff.buff);
        //itemData.weaponBuff.buff = _buff;
        //itemData.weaponBuff.duration = _duration;
        //itemData.weaponBuff.magnitude = _magnitude;
        itemData.weaponBuff = _buff;
    }
}
