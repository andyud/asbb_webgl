using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.Purchasing;
using UnityEngine.Purchasing.Security;
using System.Linq;

public enum IAPTYPE
{
	BALLSPEED = 0,
	SKIPTURN,
	ADREMOVE_BALLSPEED,
}

public class IAPController : MonoBehaviour, IStoreListener
{
	public static IAPController instance;
	public static IStoreController storeController = null;
	private static string[] strProductID;
	public const string pdSpeed = "swipe_ball_speed";
	public const string pdSkipTurn = "swipe_skipturn";
	public const string pdAdRemoveSpeed = "swipe_adremove_ballspeed";
	[HideInInspector]
	public string costSpeed = "";
	[HideInInspector]
	public string costSkipTurn = "";
	[HideInInspector]
	public string costAdRemoveSpeed = "";

	IExtensionProvider extensionProvider;
	ConfigurationBuilder builder;

	private void Awake()
	{
		instance = this;

		if (storeController == null)
		{
			strProductID = new string[] { pdSpeed, pdSkipTurn, pdAdRemoveSpeed };
			Initialize();
		}

		extensionProvider = null;
	}

	private void Initialize()
	{
		builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

		for (int i = 0; i < strProductID.Length; i++)
		{
			if(Application.platform == RuntimePlatform.Android)
				builder.AddProduct(strProductID[i], ProductType.NonConsumable, new IDs { { strProductID[i], GooglePlay.Name } });

			if(Application.platform == RuntimePlatform.IPhonePlayer)
				builder.AddProduct(strProductID[i], ProductType.NonConsumable, new IDs { { strProductID[i], AppleAppStore.Name } });
		}

		UnityPurchasing.Initialize(this, builder);
	}

	public int GetItemType(string productId)
	{
		try
		{
			var product = storeController.products.all.First(v => v.definition.id.Equals(productId));
			if(product != null && product.definition != null)
				return (int)product.definition.type;
		}
		catch(Exception e)
		{
			Debug.LogError($"=====error => IAPController.GetItemType(string productId) / message: {e}");
		}

		return -1;
	}

	// ios에서는 수동 restore 하도록..
	public void Restore(Action callback)
	{
		Debug.Log("Restore");

		if(extensionProvider == null)
		{
			if (callback != null)
				callback.Invoke();

			Debug.Log("extensionProvider is null");
			PopUpController.AddAlert(AlertPopUpType.Restore_Fail);
			return;
		}

		switch(Application.platform)
		{
			case RuntimePlatform.IPhonePlayer:
				extensionProvider.GetExtension<IAppleExtensions>().RestoreTransactions(isSuccess =>
				{
					if (isSuccess)
					{
						Debug.Log("Apple Restore is success");
					}
					else
					{
						PopUpController.AddAlert(AlertPopUpType.Restore_Fail);
						Debug.Log("Apple Restore is failed");
					}

					if(callback != null)
						callback.Invoke();
				});
				break;
			//case RuntimePlatform.Android:
			//	extensionProvider.GetExtension<IGooglePlayStoreExtensions>().RestoreTransactions(isSuccess =>
			//	{
			//		if (isSuccess)
			//		{
			//			Debug.Log("=====Google Restore is success");
			//			//UnityPurchasing.Initialize(this, builder);
			//		}
			//		else
			//		{
			//			Debug.Log("=====Google Restore is fail");
			//		}
			//		
			//		if(callback != null)
			//			callback.Invoke();
			//	});
			//	break;
		}
	}

	public void SetIAP()
	{
#if UNITY_ANDROID
		if(storeController == null || FireBaseController.isLogin)
		{
			Debug.Log($"IAPController.SetIAP() / storeController is null or FirebaseLogin is false");
			return;
		}
#endif
		var products = storeController.products.all;
		for (int i = 0; i < products.Length; i++)
		{
			if (products[i].hasReceipt)
			{
				if (products[i].definition.id.Equals(strProductID[0]))
				{
					Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[0]);

					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 1);
				}
				else if (products[i].definition.id.Equals(strProductID[1]))
				{
					Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[1]);

					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 1);
				}
				else if (products[i].definition.id.Equals(strProductID[2]))
				{
					Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[2]);

					AdController.Hide_BannerView();

					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 1);
					AdController.AdPurchase = true;
				}
			}
			else //환불에 따른 처리.
			{
				Debug.LogFormat("OnInit item name: {0}  / HasReceipt is false", products[i].definition.id);

				if (products[i].definition.id.Equals(strProductID[0]))
				{
					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 0);
				}
				else if (products[i].definition.id.Equals(strProductID[1]))
				{
					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 0);
				}
				else if (products[i].definition.id.Equals(strProductID[2]))
				{
					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 0);
					AdController.AdPurchase = false;
				}
			}

			//if (products[i].definition.id.Equals(pdSpeed))
			//	costSpeed = products[i].metadata.localizedPriceString;
			//else if (products[i].definition.id.Equals(pdSkipTurn))
			//	costSkipTurn = products[i].metadata.localizedPriceString;
			//else if (products[i].definition.id.Equals(pdAdRemoveSpeed))
			//	costAdRemoveSpeed = products[i].metadata.localizedPriceString;
		}
	}

	public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
	{
		Debug.Log("OnInitialized");
		storeController = controller;
		extensionProvider = extensions;

		var products = storeController.products.all;
		for (int i = 0; i < products.Length; i++)
		{
#if UNITY_ANDROID
			if (FireBaseController.isLogin == false)
#endif
			{
				if (products[i].hasReceipt)
				{
					if (products[i].definition.id.Equals(strProductID[0]))
					{
						Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[0]);

						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 1);
					}
					else if (products[i].definition.id.Equals(strProductID[1]))
					{
						Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[1]);

						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 1);
					}
					else if (products[i].definition.id.Equals(strProductID[2]))
					{
						Debug.LogFormat("OnInit item name: {0}  / HasReceipt is true", strProductID[2]);

						AdController.Hide_BannerView();

						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 1);
						AdController.AdPurchase = true;
					}
				}
				else //환불에 따른 처리.
				{
					Debug.LogFormat("OnInit item name: {0}  / HasReceipt is false", products[i].definition.id);

					if (products[i].definition.id.Equals(strProductID[0]))
					{
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 0);
					}
					else if (products[i].definition.id.Equals(strProductID[1]))
					{
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 0);
					}
					else if (products[i].definition.id.Equals(strProductID[2]))
					{
						PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 0);
						AdController.AdPurchase = false;
					}
				}
			}

			if (products[i].definition.id.Equals(pdSpeed))
				costSpeed = products[i].metadata.localizedPriceString;
			else if (products[i].definition.id.Equals(pdSkipTurn))
				costSkipTurn = products[i].metadata.localizedPriceString;
			else if (products[i].definition.id.Equals(pdAdRemoveSpeed))
				costAdRemoveSpeed = products[i].metadata.localizedPriceString;
		}
	}

	public void OnInitializeFailed(InitializationFailureReason error)
	{
		Debug.LogFormat("failed OnInit / errorReason: {0}", error.ToString());
	}

	public void OnInitializeFailed(InitializationFailureReason error, string? message)
	{
		Debug.LogFormat("failed OnInit / errorReason: {0}", error.ToString());
	}

	public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
	{
		Debug.LogFormat("failed Purchase / errorReason: {0}", p.ToString());
	}

	public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
	{
		Debug.LogFormat("ProcessPurchase / eventArgs.hasReceipt: {0} / eventArgs.availableToPurchase: {1} / eventArgs.transationID: {2} / eventArgs.id: {3}", e.purchasedProduct.hasReceipt, e.purchasedProduct.availableToPurchase, e.purchasedProduct.transactionID, e.purchasedProduct.definition.id);
		bool isSuccess = true;

		CrossPlatformValidator validator = new CrossPlatformValidator(GooglePlayTangle.Data(), AppleTangle.Data(), Application.identifier);

		try
		{
			IPurchaseReceipt[] result = validator.Validate(e.purchasedProduct.receipt);
			for (int i = 0; i < result.Length; i++)
			{
				Analytics.Transaction(result[i].productID, e.purchasedProduct.metadata.localizedPrice, e.purchasedProduct.metadata.isoCurrencyCode, result[i].transactionID, null);
			}
		}
		catch (IAPSecurityException iapSecurityException)
		{
			Debug.LogFormat("Exception => IAPSecurityException: {0}", iapSecurityException.Message);
			isSuccess = false;
		}

		if (isSuccess)
		{
			Debug.Log("ProcesPurchase is Success");
#if UNITY_ANDROID
			if(FireBaseController.isLogin)
			{
				AppServerController.Instance.IAP(e.purchasedProduct.receipt, ConfirmReceipt =>
				{
					var storePopUp = FindObjectOfType<PopUp_Store>();
					if (storePopUp != null)
						storePopUp.SetUI();

					PurchaseProcessingComplete(ConfirmReceipt);
				}, false);
			}
			else
#endif
			{
				if (e.purchasedProduct.definition.id.Equals(strProductID[0]))
				{
					Debug.LogFormat("ProcessPurchase item name: {0} is Purchased", strProductID[0]);

					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Speed, 1);

					InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
					InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
				}
				else if (e.purchasedProduct.definition.id.Equals(strProductID[1]))
				{
					Debug.LogFormat("ProcessPurchase item name: {0} is Purchased", strProductID[1]);

					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Continue, 1);
				}
				else if (e.purchasedProduct.definition.id.Equals(strProductID[2]))
				{
					Debug.LogFormat("ProcessPurchase item name: {0} is Purchased", strProductID[2]);
					PlayerPrefs.SetInt(PlayerPrefs_Config.Purchase_Total, 1);

					AdController.Hide_BannerView();
					AdController.AdPurchase = true;

					InGameController._instance._shooter.IsHandleUserEarnedRewardSpeedCallback = true;
					InGameController._instance._shooter.IsHandleUserEarnedRewardPowerModeCallback = true;
				}
			}
		}
		else
		{
			Debug.Log("ProcesPurchase is Failed");
			return PurchaseProcessingResult.Pending;
		}
#if UNITY_ANDROID
		if(FireBaseController.isLogin)
		{
			return PurchaseProcessingResult.Pending;
		}
		else
#endif
		{
			var storePopUp = FindObjectOfType<PopUp_Store>();
			if (storePopUp != null)
				storePopUp.SetUI();
			return PurchaseProcessingResult.Complete;
		}
	}

	public void PurchaseProcessingComplete(string receipt)
	{
		if (storeController == null)
		{
			Debug.LogError($"error => IAPController.PurchaseProcessingComplete()");
			return;
		}
		
		try
		{
			var product = storeController.products.all.First(v => v.receipt == receipt);
			storeController.ConfirmPendingPurchase(product);
		}
		catch(Exception e)
		{
			Debug.LogException(e);
		}
	}

	public void Purchase(IAPTYPE index)
	{
		Debug.LogFormat("Purchase IAPTYPE: {0}", index.ToString());

		if (storeController == null)
		{
			Debug.Log("Purchase method storeController is null");
		}
		else
		{
#if UNITY_ANDROID
			if (FireBaseController.isLogin)
			{
				storeController.InitiatePurchase(strProductID[(int)index]);
			}
			else
#endif
			{
				PopUpController.CloseCurrentPopUp();

				StopCoroutine("OpenLoginPopup");
				StartCoroutine("OpenLoginPopup");
			}
		}
	}

	IEnumerator OpenLoginPopup()
	{
		yield return new WaitUntil(() => PopUpController._IsPopUpOpened == false);
		PopUpController.Open_Login();
	}
}