//
//  IUILifeCycle.cs 
//
//  Author: HGT 
//
//  Copyright (c) 2021 hegametech.com 
//

namespace CustomGameFramework.Runtime
{
    public interface IUILifeCycle
    {
        void Init(string UIEventKey, int id = 0);
        
        void Dispose();
    }
}
