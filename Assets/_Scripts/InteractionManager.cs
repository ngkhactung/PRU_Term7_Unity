using UnityEngine;

public class InteractionManager : MonoBehaviour
{
    public static InteractionManager Instance { get; set; }

    public GameObject hoveredWeapon = null;

    public GameObject hoveredAmmoBox = null;

    public GameObject hoveredItem = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Update()
    {
        Ray ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit))
        {
            GameObject objectHitByRay = hit.transform.gameObject;

            if (objectHitByRay.GetComponent<Weapon>() || objectHitByRay.GetComponent<AmmoBox>())
            {
                hoveredItem = objectHitByRay;
                hoveredItem.GetComponent<Outline>().enabled = true;

                if (Input.GetKeyDown(KeyCode.F)) PickUpItem(hoveredItem);
            }
            else
            {
                if (hoveredItem != null) hoveredItem.GetComponent<Outline>().enabled = false; hoveredItem = null;
            }
        }
    }

    private void PickUpItem(GameObject hoveredItem)
    {
        if (hoveredItem.GetComponent<Weapon>())
        {
            WeaponManager.Instance.PickUpWeapon(hoveredItem);
        }
        else if (hoveredItem.GetComponent<AmmoBox>())
        {
            WeaponManager.Instance.PickUpAmmoBox(hoveredItem);
        }
    }
}
