﻿using DalApi;
namespace Helpers;

internal static class CallManager
{
    private static IDal s_dal = Factory.Get; //stage 4
    // כל המתודות במחלקה יהיו internal static
}
