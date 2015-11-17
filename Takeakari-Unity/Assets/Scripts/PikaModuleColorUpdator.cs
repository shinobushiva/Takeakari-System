using UnityEngine;
using System.Collections;
using System;
using System.Collections.Generic;

public class PikaModuleColorUpdator : ModuleColorUpdator {

    private bool init = false;

    public override int ModulesToBuffer()
    {
        if (!init)
        {
            print(modules.Length);
            buf = new byte[modules.Length*3+1];
            init = true;
        }
        buf [0] = 0x7f;

        for(int i=0;i<modules.Length;i++){
            Color col = modules[i].light.color;
            buf[i*3+1] = (byte)(((byte)(col.r * 127)) | 0x80);
            buf[i*3+1+1] = (byte)(((byte)(col.g * 127)) | 0x80);
            buf[i*3+2+1] = (byte)(((byte)(col.b * 127)) | 0x80);

        }

        /*
        int idx = 0;
        int counter = 0;
        foreach (Module m in modules)
        {
            counter++;
            if (counter % 8 == 0)
            {
                counter++;
            }
            if (cp.useCompress && colorMap[m] == m.light.color)
                continue;
            if (m == null)
                continue;
            Color col = m.light.color;
            colorMap[m] = col;
            buf[idx++] = 0x3F;
            buf[idx++] = (byte)(((byte)(col.r * 127)) | 0x80);
            buf[idx++] = (byte)(((byte)(col.g * 127)) | 0x80);
            buf[idx++] = (byte)(((byte)(col.b * 127)) | 0x80);
            //if(m.moduleId > 0){
            //    buf [idx++] = ((byte)(((byte)m.moduleId) | 0x80));
            //}else{
            //print(""+counter);
            buf[idx++] = ((byte)(((byte)counter) | 0x80));
            //}
        }
        */
        return buf.Length;
    }

}
