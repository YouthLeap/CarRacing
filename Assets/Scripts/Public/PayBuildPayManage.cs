// #define FreePay 

using UnityEngine;
using System.Collections;
using UnityEngine.Purchasing;
using System.Collections.Generic;

namespace PayBuild
{


    public class PayBuildPayManage : MonoBehaviour, IStoreListener
    {
        private IStoreController controller;
        private IExtensionProvider extensions;
        string initIAPFailMessage = "";
        static string packagePrefix = "com.product.name.package";
        static int totalPackage = 15;
        static int removeAdProductNumber = 1000;

        private static PayBuildPayManage instance;
        public static PayBuildPayManage Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = GameObject.FindObjectOfType<PayBuildPayManage>();
                    if (instance == null)
                    {
                        GameObject payManage = new GameObject("PayManage");
                        instance = payManage.AddComponent<PayBuildPayManage>();
                        DontDestroyOnLoad(payManage);
                        instance.JustInit();
                    }
                }
                return instance;
            }
        }



        public void JustInit()
        {
            //Debug.LogError("IAP JustInit");

            var builder = ConfigurationBuilder.Instance(StandardPurchasingModule.Instance());

            for (int i = 1; i <= totalPackage; i++)
            {
                //Debug.Log("pak:" + packagePrefix + i.ToString());
                builder.AddProduct(packagePrefix + i.ToString(), ProductType.Consumable);
            }

            builder.AddProduct(packagePrefix + removeAdProductNumber, ProductType.NonConsumable);

            builder.Configure<IGooglePlayConfiguration>().SetPublicKey("MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAnClUOxKqp+tqdaxLrlT5W8m/TK/iTmpcE1BfsT5a2nLPtVAFLj9ElY5VhwToIQjH8YIUgnt88+g1RsyMh6+ngwpAiduqIh0jFxztiy0xqA+N3PuZmb94Kb98xSzE5B+wWBo4FPyqARfa2ONsP7R00oazWv9t1dU6yVWEswhVNSBCZI6C0C0r5VnKeURdnvVhh51eVpRgJojuEvTwNmR5XiN/kwfKwXZBDcT4SWSoiH9FfILBuLhD9IF5KetDjqXPRT4Ju+CeSE3xcE31Nl5IUEYbTWtRlfGf0z3dtqt8HNQ1CphN+7G/1l/kCShC8CzZPkF3WntcryUWN7mCv8ETiwIDAQAB");

            UnityPurchasing.Initialize(this, builder);
        }

        #region unity call back
        public void OnInitialized(IStoreController controller, IExtensionProvider extensions)
        {
            //Debug.Log("OnInitialized IAP");
            this.controller = controller;
            this.extensions = extensions;
        }

        public void OnInitializeFailed(InitializationFailureReason error)
        {
            initIAPFailMessage = error.ToString();
            Debug.LogError("OnInitializeFailed IAP => " + error.ToString());

        }

        public void OnPurchaseFailed(Product i, PurchaseFailureReason p)
        {
            Debug.LogError("OnPurchaseFailed IAP => " + p.ToString());

            CallBack("Failed");

        }

        public PurchaseProcessingResult ProcessPurchase(PurchaseEventArgs e)
        {
            Debug.Log("ProcessPurchase IAP => " + (e.purchasedProduct.receipt != null ? e.purchasedProduct.receipt : "null"));
           
            CallBack("Success");
            return PurchaseProcessingResult.Complete;
        }
        #endregion

        private bool IsInitialized()
        {
            // Only say we are initialized if both the Purchasing references are set.
            return controller != null && extensions != null;
        }

        string GetProductId(int PayIndex)
        {
            if(PayIndex == removeAdProductNumber) // remove ad
            {
                return packagePrefix + removeAdProductNumber;
            }

            if(PayIndex < 1 || PayIndex > totalPackage)
            {
                Debug.LogError("Unknown payindex:" + PayIndex);
                return "";
            }

            return packagePrefix + PayIndex;
        }


		public string GetProductPriceString(int payIndex)
		{
			if (controller == null || controller.products == null) 
			{
				Debug.LogWarning ("Price not yet available! " + GetProductId(payIndex));
				return "0.00 $";
			}

			var meta = controller.products.WithID(GetProductId(payIndex)).metadata;
			return string.Format ("{0} {1}", meta.localizedPrice, meta.isoCurrencyCode);
		}

		public float GetProductPrice(int payIndex)
		{
			if (controller == null || controller.products == null) 
			{
				Debug.LogWarning ("Price not yet available! " + GetProductId(payIndex));
				return 0f;
			}

			var meta = controller.products.WithID(GetProductId(payIndex)).metadata;

			Debug.LogWarning(GetProductId(payIndex)
				+ "\nisoCurrencyCode = " + meta.isoCurrencyCode
				+ "\nlocalizedDescription =" + meta.localizedDescription
				+ "\nlocalizedPrice = " + meta.localizedPrice
				+ "\nlocalizedPriceString = " + meta.localizedPriceString
				+ "\nlocalizedTitle = " + meta.localizedTitle
			);

			return (float)meta.localizedPrice;
		}

        public delegate void PayCallBack(string result);
        event PayCallBack payCallBack;
        int payIndex = 1;

        public void Pay(int PayIndex, PayCallBack paycallback)
        {
            payCallBack = paycallback;
            payIndex = PayIndex;

            string productId = GetProductId(PayIndex);

            Debug.Log("Start Pay [code:" + PayData.Instance.GetPayCode(PayIndex) + "][desc:" + PayData.Instance.GetDesc(PayIndex) + "][productId:" + productId + "]");

            // If Purchasing has been initialized ...
            if (IsInitialized())
            {
                // ... look up the Product reference with the general product identifier and the Purchasing 
                // system's products collection.
                Product product = controller.products.WithID(productId);

                // If the look up found a product for this device's store and that product is ready to be sold ... 
                if (product != null && product.availableToPurchase)
                {
                    Debug.Log(string.Format("Purchasing product asychronously: '{0}'", product.definition.id));
                    // ... buy the product. Expect a response either through ProcessPurchase or OnPurchaseFailed 
                    // asynchronously.
                    controller.InitiatePurchase(product);
                }
                // Otherwise ...
                else
                {
                    // ... report the product look-up failure situation  
                    Debug.Log("BuyProductID: FAIL. Not purchasing product, either is not found or is not available for purchase");

                    CallBack("Failed");

                    return;
                }
            }
            // Otherwise ...
            else
            {

                Debug.Log("BuyProductID FAIL IAP");

                CallBack("Failed");
            }

        }

        public void Restore(int PayIndex, PayCallBack paycallback)
        {
            payCallBack = paycallback;

            //#if UNITY_EDITOR_OSX || UNITY_EDITOR
            //            IsUnityDelay = true;
            //            return;
            //#endif

            //#if UNITY_IOS
            //	//		if(Application.loadedLevelName.CompareTo("UIScene") == 0)
            //	//		{
            //	//			WaitingBox.Instance.Show();
            //	//		}
            //	//		else
            //	//		{
            //	//			GameWaitingBox.Instance.Show();
            //	//		}
            //	//		PayBuildIOS.Instance.Restore(PayBuildPayCodeData.Instance.GetPayCode(PayIndex), gameObject.name, "CallBack");
            //#endif
        }

        /// <summary>
        /// Call the third party deductive plugin to unify the return function
        /// </summary>
        /// <param name="result">Result.</param>
        void CallBack(string result)
        {
           
            if (payCallBack != null)
            {
                payCallBack(result);
                payCallBack = null;
            }

            PlayerData.Instance.SaveData();

            string localStr = (GlobalConst.SceneName == SceneType.UIScene ? "关卡外" : "关卡内");

            if (result.CompareTo("Success") == 0)
            {
                CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Pay_Success,
                    "位置情况", localStr + "_Level_" + PlayerData.Instance.GetSelectedLevel(),
                    "购买价格", PayData.Instance.GetCost(payIndex).ToString(),
                    "扣费点名称", PayData.Instance.GetDesc(payIndex).ToString());

                int paysource = 1;
                if (PlatformSetting.Instance.PlatformType == PlatformItemType.YiDong)
                    paysource = 5;
                else if (PlatformSetting.Instance.PlatformType == PlatformItemType.LianTong)
                    paysource = 6;
                else if (PlatformSetting.Instance.PlatformType == PlatformItemType.DianXin)
                    paysource = 7;

                CollectInfoEvent.PayEvent(PayData.Instance.GetCost(payIndex), PayData.Instance.GetPayTypeStr(payIndex), 1, 0, paysource);

            }
            else
                CollectInfoEvent.SendEvent(CollectInfoEvent.EventType.Pay_Fail,
                    "位置情况", localStr + "_Level_" + PlayerData.Instance.GetSelectedLevel(),
                    "购买价格", PayData.Instance.GetCost(payIndex).ToString(),
                    "扣费点名称", PayData.Instance.GetDesc(payIndex).ToString());

        }

    }

}