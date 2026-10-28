using UnityEngine;

public class ShopInteractable : MonoBehaviour, IInteractable
{
    public void Interact()
    {
        ShopManager shopManager = FindFirstObjectByType<ShopManager>();
        if (shopManager != null)
        {
            shopManager.OpenShop();
        }
        else
        {
            Debug.LogWarning("No se encontró ningún ShopManager en la escena.");
        }
    }
}