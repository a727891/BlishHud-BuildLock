﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;
using Gw2Sharp.WebApi.V2.Models;


namespace BoonsUp.Services;

public static class AccountNameService
{
    public static string DEFAULT_ACCOUNT_NAME = "default";
    public static async Task<string> UpdateAccountName()
    {
        var gw2ApiManager = Service.Gw2ApiManager;

        if (gw2ApiManager.HasPermissions(NecessaryApiTokenPermissions) == false)
        {

            return DEFAULT_ACCOUNT_NAME;
        }

        try
        {
            var accountInfo = await gw2ApiManager.Gw2ApiClient.V2.Account.GetAsync();

            return accountInfo.Name;
        }
        catch (Exception e)
        {
            Debug.WriteLine("BoonsUp: Warning, account name not received from the API " + e.Message);
            return DEFAULT_ACCOUNT_NAME;
        }
    }

    public static readonly List<TokenPermission> NecessaryApiTokenPermissions = new()
    {
        TokenPermission.Account
    };
}