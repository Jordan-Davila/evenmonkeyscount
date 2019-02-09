using UnityEngine;
using EasyMobile;

public class AdsManager : MonoBehaviour 
{
    public GameManager gm;

    private void Awake() 
    {
        gm = Object.FindObjectOfType<GameManager>();
    }

	private void Start() 
    {
        Init();
    }

    private void Init()
    {
        // Grants the module-level consent for the Advertising module.
        Debug.LogWarning("Init Ads");
        Advertising.GrantDataPrivacyConsent();
    }

    public void ShowBannerAd()
    {
        if (gm.ads)
            Advertising.ShowBannerAd(BannerAdPosition.Bottom);    
        
    }

    public void DestroyBannerAd()
    {
        if (gm.ads)
        {
            Advertising.HideBannerAd();
            Advertising.DestroyBannerAd();
        }
    }
}