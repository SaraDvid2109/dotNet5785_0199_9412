﻿namespace DalApi;
public interface IConfig
{
    DateTime Clock { get; set; }
    void Reset();
   TimeSpan RiskRange { get; set; }
}
