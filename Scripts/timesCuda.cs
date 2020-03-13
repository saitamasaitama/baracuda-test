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
    // Start is called before the first frame update
    void Start()
    {
        model = ModelLoader.Load(modelSource);
        worker = BarracudaWorkerFactory.CreateWorker(BarracudaWorkerFactory.Type.ComputePrecompiled, model);
        
        Texture2D tex = (Texture2D)Resources.Load("img_7"); // これ32*32で読み込んでしまうので修正。
        var pixel = tex.GetPixels(0, 0, 28, 28);
        var tex2 = new Texture2D(28, 28); 
        tex2.SetPixels(pixel);
        tex2.Apply();
        inputs["0"] = new Tensor(tex2, 1); 
        worker.Execute(inputs["0"]);
        Tensor output = worker.PeekOutput();
        Debug.Log(output.GetType());// Barracuda.Tensor型
        Debug.Log(output.data.GetMaxCount()); //
        for(int i = 0; i < 10; i++)
        {
            Debug.Log(output[i]);
        }
        output.Dispose(); // Dispose()で終了させないとダメらしい
        worker.Dispose();
        Debug.Log("end");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
