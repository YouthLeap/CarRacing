using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Text;

public class  LotteryWinnerDisplay : MonoBehaviour {
	
	public int PhoneNumAmount = 10000;
	public List<EasyFontTextMesh> TextList;
	public float TextLength = 320f;
	public float FirstTextX = 450f;
	public float TextHideX = 600f;
	public float Speed = 50f;

    /*Display the data of the configuration The original turntable can be used */
    public string getText="";
	public List<string> resultTextList;
	public List<int> weightList;

	void OnEnable()
	{

		for(int i = 0; i < TextList.Count; i ++)
		{
			TextList[i].transform.localPosition = new Vector3(FirstTextX + TextLength * i, TextList[i].transform.localPosition.y, TextList[i].transform.localPosition.z);

			if(TextList[i] != null)
            {
                TextList[i].text = GetRandomWinner();                
            }
			    
		}
	}

	void Update()
	{
		for(int i = 0; i < TextList.Count; i ++)
		{
			if(TextList[i].transform.localPosition.x <= (TextHideX - TextList.Count * TextLength))
			{
				TextList[i].transform.localPosition = new Vector3(TextHideX, TextList[i].transform.localPosition.y, TextList[i].transform.localPosition.z);

				if(TextList[i] != null)
				    TextList[i].text = GetRandomWinner();
			}

			TextList[i].transform.Translate(Vector3.left * Time.deltaTime * Speed);
		}
	}

	string GetRandomWinner()
	{
        return "";

		string num = RandomPhoneNumData.Instance.GetPhoneNumber(Random.Range(1, PhoneNumAmount +1));

		while(num == null || num.Length != 11)
			num = RandomPhoneNumData.Instance.GetPhoneNumber(Random.Range(1, PhoneNumAmount +1));


        /*Show the original data*/
        if (getText=="" || resultTextList.Count<1 ||  resultTextList.Count !=weightList.Count)
		{
				string count;
				int countRange = Random.Range (1, 11);
				if(countRange < 7)
					count = "10";
				else if(countRange < 10)
					count = "20";
				else
					count = "50";

				return "Congratulations " + num + " obtain " + count + "";
		}else
		{
            /*Displays the configured data*/
            int totalWeight =0;
			for(int i=0;i<weightList.Count;++i)
			{
				totalWeight+=weightList[i];
			}
			int countRange = Random.Range (0, totalWeight+1);
			int upLimit=0;
			string resString = resultTextList[0];

			for(int j=0;j<weightList.Count;++j)
			{
				upLimit+=weightList[j];
				if(countRange<=upLimit)
				{
					resString = resultTextList[j];
					break;
				}
			}

			return "Congratulations " + num +getText+resString;
		}
	}
}
