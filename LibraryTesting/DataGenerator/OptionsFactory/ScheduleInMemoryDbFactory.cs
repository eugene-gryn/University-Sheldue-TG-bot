﻿using System;
using DAL.EF;
using Microsoft.EntityFrameworkCore;

namespace LibraryTesting.DataGenerator.OptionsFactory;

public class ScheduleInMemoryDbFactory : ContextFactory<ScheduleContext>
{
    public ScheduleInMemoryDbFactory() : base("")
    {
    }

    public override DbContextOptions<ScheduleContext> CreateOptions()
    {
        return new DbContextOptionsBuilder<ScheduleContext>()
            .EnableSensitiveDataLogging()
            .UseInMemoryDatabase("Schedule Db")
            .Options;
    }
}