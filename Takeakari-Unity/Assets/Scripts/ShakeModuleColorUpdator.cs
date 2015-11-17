using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class ShakeModuleColorUpdator : ModuleColorUpdator {

    private bool init = false;

    public WebSocketConnect wsc;

    private int prev = 0;

    public int ledNum = 60;
    
    public override int ModulesToBuffer()
    {
        if (!init)
        {
            print(modules.Length);
            buf = new byte[modules.Length*3+2];
            init = true;
        }
        buf [0] = 0x3f;

        int val = Mathf.RoundToInt(Mathf.Lerp(prev, Mathf.Clamp(wsc.value*ledNum, 0, ledNum), 0.5f));
        prev = val;
        
        for(int i=0;i<modules.Length;i++){
            Color col = modules[i].light.color;
            buf[i*3+1] = (byte)((byte)(val) | 0x80);
            buf[i*3+1+1] = (byte)(((byte)(val/(float)ledNum*100)) | 0x80);
            buf[i*3+2+1] = (byte)0x80;

            buf[i*3+3+1] = ((byte)(((byte)modules[i].moduleId) | 0x80));
            
        }
  
        return buf.Length;
    }


}
