using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace FinanceManager.Helpers;

public static class ServiceHelper
{
    public static IServiceProvider Services { get; private set; } = default!;

    public static void Initialize(IServiceProvider services) => Services = services;

    public static T Get<T>() where T : notnull => Services.GetRequiredService<T>();
}

