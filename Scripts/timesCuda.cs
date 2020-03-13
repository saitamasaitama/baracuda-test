using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Barracuda;

public class timesCuda : MonoBehaviour
{
    public NNModel modelSource;
    private Model model;
    private IWorker worker;
    Dictionary<string, Tensor> inputs = new Dictionary<string, Tensor>();
     private IEnumerator ExecuteModel()
    {
        Debug.Log("Go");
        // Create input and Execute model
        yield return worker.ExecuteAsync(inputs);

        // Get outputs


    }

    // Start is called before the first frame update
    void Start()
    {
        model = ModelLoader.Load(modelSource);
        worker = BarracudaWorkerFactory.CreateWorker(BarracudaWorkerFactory.Type.ComputePrecompiled, model);
        
        Texture2D tex = (Texture2D)Resources.Load("img_7");
        var pixel = tex.GetPixels(0, 0, 28, 28);
        var tex2 = new Texture2D(28, 28); 
        tex2.SetPixels(pixel);
        tex2.Apply();
        inputs["0"] = new Tensor(tex2, 1);
        worker.Execute(inputs["0"]);
        
        var O = worker.PeekOutput();
        Debug.Log(O.GetType());// [1, 1, 1, 10]
        Debug.Log(O.data.GetMaxCount());
        var o = O.data.Download(10);// [ -1, -1, ]
        int maxI = -1;
        float max = -1000;
        for(int i =0; i < 10; i++)
        {
            //Debug.Log(o[i]);
            if(max < o[i])
            {
                maxI = i;
                max = o[i];
            }
        }
        Debug.Log($"predict: {maxI}");
        O.Dispose();
        worker.Dispose();
        Debug.Log("end");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
