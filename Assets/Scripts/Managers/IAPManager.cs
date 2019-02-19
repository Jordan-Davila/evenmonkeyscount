using UnityEngine;
using EasyMobile;
using System.Collections;

public class IAPManager : MonoBehaviour 
{
    private GameManager gm;

    private void Awake() 
    {
        gm = Object.FindObjectOfType<GameManager>();       
    }

    void OnEnable()
    {
        InAppPurchasing.PurchaseCompleted += OnPurchaseCompleted;
        InAppPurchasing.PurchaseFailed += OnPurchaseFailed;
        InAppPurchasing.RestoreCompleted += OnRestoreCompleted;
    }

    void OnDisable()
    {
        InAppPurchasing.PurchaseCompleted -= OnPurchaseCompleted;
        InAppPurchasing.PurchaseFailed -= OnPurchaseFailed;
        InAppPurchasing.RestoreCompleted -= OnRestoreCompleted;
    }

    // Successful purchase handler
    void OnPurchaseCompleted(IAPProduct product)
    {
        // Compare product name to the generated name constants to determine which product was bought
        switch (product.Name)
        {
            case EM_IAPConstants.Product_No_Ads:
                gm.RemoveAds();
                break;
            case EM_IAPConstants.Product_Theme_Pack:
                gm.UnlockAllThemes();
                break;
        }
    }

    void OnRestoreCompleted()
    {
        StartCoroutine(CROnRestoreCompleted());
        gm.RemoveAds();
    }

    IEnumerator CROnRestoreCompleted()
    {
        while (NativeUI.IsShowingAlert())
            yield return new WaitForSeconds(0.5f);

        NativeUI.Alert("Restore Completed", "Your purchases have been restored successfully.");
    }

    // Failed purchase handler
    void OnPurchaseFailed(IAPProduct product)
    {
        Debug.Log("The purchase of product " + product.Name + " has failed.");
    }
    
    public void BuyNoAds()
	{
        InAppPurchasing.Purchase(EM_IAPConstants.Product_No_Ads);
	}

    public void RestoreNoAds()
    {
        InAppPurchasing.RestorePurchases();
    }
}