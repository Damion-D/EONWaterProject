using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetupClipboard : MonoBehaviour
{
    private void Reset()
    {
        TitrationScenario scenario = GameObject.Find("Managers").transform.Find("StoryManager").GetComponent<TitrationScenario>();
        int rowCount = transform.Find("TextRows").childCount;
        scenario.textMeshPro_Row = new Transform[rowCount];
        scenario.textMeshPro_date = new Transform[rowCount];
        scenario.textMeshPro_time = new Transform[rowCount];
        scenario.textMeshPro_freeChlorine = new Transform[rowCount];
        scenario.textMeshPro_totalChlorine = new Transform[rowCount];
        scenario.textMeshPro_freeToTotal = new Transform[rowCount];
        scenario.textMeshPro_monoChlorine = new Transform[rowCount];
        scenario.textMeshPro_diChlorine = new Transform[rowCount];
        scenario.textMeshPro_monoToTotal = new Transform[rowCount];
        scenario.textMeshPro_Ammonia = new Transform[rowCount];

        for (int i = 0; i < rowCount; i++)
        {
            scenario.textMeshPro_Row[i] = transform.Find("TextRows").GetChild(i);

            scenario.textMeshPro_date[i] = scenario.textMeshPro_Row[i].GetChild(0);
            scenario.textMeshPro_time[i] = scenario.textMeshPro_Row[i].GetChild(1);
            scenario.textMeshPro_freeChlorine[i] = scenario.textMeshPro_Row[i].GetChild(2);
            scenario.textMeshPro_totalChlorine[i] = scenario.textMeshPro_Row[i].GetChild(3);
            scenario.textMeshPro_freeToTotal[i] = scenario.textMeshPro_Row[i].GetChild(4);
            scenario.textMeshPro_monoChlorine[i] = scenario.textMeshPro_Row[i].GetChild(5);
            scenario.textMeshPro_diChlorine[i] = scenario.textMeshPro_Row[i].GetChild(6);
            scenario.textMeshPro_monoToTotal[i] = scenario.textMeshPro_Row[i].GetChild(7);
            scenario.textMeshPro_Ammonia[i] = scenario.textMeshPro_Row[i].GetChild(8);


            scenario.textMeshPro_date[i].name = scenario.textMeshPro_date[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_time[i].name = scenario.textMeshPro_time[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_freeChlorine[i].name = scenario.textMeshPro_freeChlorine[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_totalChlorine[i].name = scenario.textMeshPro_totalChlorine[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_freeToTotal[i].name = scenario.textMeshPro_freeToTotal[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_monoChlorine[i].name = scenario.textMeshPro_monoChlorine[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_diChlorine[i].name = scenario.textMeshPro_diChlorine[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_monoToTotal[i].name = scenario.textMeshPro_monoToTotal[i].name.Replace("1", i.ToString());
            scenario.textMeshPro_Ammonia[i].name = scenario.textMeshPro_Ammonia[i].name.Replace("1", i.ToString());
        }
    }
}
