﻿namespace VkCelebrationApp.BLL.Configuration
{
    public interface IBotConfiguration
    {
        string Url { get; set; }

        string UpdateBaseApiPath { get; set; }

        string Key { get; set; }
    }
}
