using UnityEngine;
using System.Collections;

public class PayUIConfigurator{
	public delegate void PayUIConfigCallHander<T>(T t);
	public static event PayUIConfigCallHander<bool> CancelBtConfigCall;

	public static void PayUIConfig(PayType payType, EasyFontTextMesh Hint1, EasyFontTextMesh Hint2, EasyFontTextMesh Desc1, EasyFontTextMesh Desc2, EasyFontTextMesh GetBt,tk2dSprite CloseBt, BoxCollider btArea, PayUIConfigCallHander<bool> cancelBtConfigCall = null)
	{
		CancelBtConfigCall = cancelBtConfigCall;

		string desc, btText;
		string costString = PayBuild.PayBuildPayManage.Instance.GetProductPriceString((int)payType);

		if(PayJsonData.Instance.GetIsJsonCtrlBt())
		if (PlatformSetting.Instance.PayVersionType == PayVersionItemType.ShenHe || string.IsNullOrEmpty(PlatformSetting.Instance.PayButtonText) || PlatformSetting.Instance.PayButtonText == "0")
			btText = PayJsonData.Instance.GetButtonText(payType);
		else
			btText = PlatformSetting.Instance.PayButtonText;
		else 
			btText = PayData.Instance.GetPayBtName(payType);
		
		desc = PayJsonData.Instance.GetHintText1(payType);
		
		if(PayJsonData.Instance.GetHintTextDependOnBt())
		{
			desc = "\"" + btText + "\"" + desc;
		}

		if (GetBt != null) {
			GetBt.text = btText;
			GetBt.text = costString;//GetBt.text.Replace("￥", cost.ToString());
		}

		if (Desc1 != null) {
			Desc1.gameObject.SetActive(PayJsonData.Instance.GetDescText1ActiveState(payType));
			Desc1.text = PayJsonData.Instance.GetDescText1(payType);
		}

		if (Desc2 != null) {
			Desc2.gameObject.SetActive(PayJsonData.Instance.GetDescText2ActiveState(payType));
			Desc2.text = PayJsonData.Instance.GetDescText2(payType);
		}

		if (Hint1 != null) {
			Hint1.gameObject.SetActive(PayJsonData.Instance.GetHintText1ActiveState(payType));
			Hint1.text = desc;
			Hint1.FontSize = PayJsonData.Instance.GetHint1FontSize(payType);
			Hint1.Size = Hint1.FontSize;
			Hint1.FontColorTop = PayJsonData.Instance.GetHintText1Color(payType);
			Hint1.FontColorBottom = PayJsonData.Instance.GetHintText1Color(payType);
			if(Hint1.EnableShadow)
				Hint1.ShadowColor = PayJsonData.Instance.GetHintText1ShadowColor(payType);
			if(Hint1.EnableOutline)
				Hint1.OutlineColor = PayJsonData.Instance.GetHintText1OutlineColor(payType);

			//Debug.Log ("Hint1 : " + Hint1.text);
			Hint1.text = Hint1.text.Replace("￥ usd", costString);
		}

		if (Hint2 != null) {
			Hint2.gameObject.SetActive(PayJsonData.Instance.GetHintText2ActiveState(payType));
			Hint2.text = PayJsonData.Instance.GetHintText2(payType);
			Hint2.FontSize = PayJsonData.Instance.GetHint2FontSize(payType);
			Hint2.Size = Hint2.FontSize;
			Hint2.FontColorTop = PayJsonData.Instance.GetHintText2Color(payType);
			Hint2.FontColorBottom = PayJsonData.Instance.GetHintText2Color(payType);
			if(Hint2.EnableShadow)
				Hint2.ShadowColor = PayJsonData.Instance.GetHintText2ShadowColor(payType);
			if(Hint2.EnableOutline)
				Hint2.OutlineColor = PayJsonData.Instance.GetHintText2OutlineColor(payType);

			//Debug.Log ("Hint2 : " + Hint2.text);
			Hint2.text = Hint2.text.Replace("￥ usd", costString);
		}

		if (CloseBt != null) {
			CloseBt.SetSprite(PayJsonData.Instance.GetCloseBtSprite(payType));
			CloseBt.scale = PayJsonData.Instance.GetCloseBtScale(payType);
		}

		if (btArea != null)
			btArea.size = PayJsonData.Instance.GetNowBuyOnClickArea (payType);

		if(CancelBtConfigCall != null)
			CancelBtConfigCall(PayJsonData.Instance.GetNeedShowCancelBt(payType));
	}
}
