﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace SmartCode.Configuration.ConfigBuilders
{
    public abstract class ConfigBuilder : IConfigBuilder
    {
        public string ConfigPath { get; }
        public Project Project { get; set; }

        protected ConfigBuilder(string configPath)
        {
            ConfigPath = configPath;
        }

        public Project Build()
        {
            using (StreamReader configStream = new StreamReader(ConfigPath))
            {
                var jsonConfigStr = configStream.ReadToEnd();
                Project = Deserialize(jsonConfigStr);
            }

            InitDefault();
            return Project;
        }

        protected abstract Project Deserialize(string content);

        private void InitDefault()
        {
            if (Project.Output != null)
            {
                if (String.IsNullOrEmpty(Project.Output.Type))
                {
                    Project.Output.Type = "File";
                }

                if (Project.Output?.Mode == CreateMode.None)
                {
                    Project.Output.Mode = CreateMode.Incre;
                }
            }

            if (Project.NamingConverter == null)
            {
                Project.NamingConverter = NamingConverter.Default;
            }

            foreach (var buildTask in Project.BuildTasks.Values)
            {
                if (buildTask.Output != null)
                {
                    if (String.IsNullOrEmpty(buildTask.Output.Type))
                    {
                        buildTask.Output.Type = Project.Output.Type;
                    }

                    if (buildTask.Output.Mode == CreateMode.None)
                    {
                        buildTask.Output.Mode = Project.Output.Mode;
                    }

                    if (!buildTask.Output.DotSplit.HasValue)
                    {
                        buildTask.Output.DotSplit = Project.Output.DotSplit;
                    }
                }

                if (buildTask.TemplateEngine == null)
                {
                    buildTask.TemplateEngine = Project.TemplateEngine;
                }
                else
                {
                    if (String.IsNullOrEmpty(buildTask.TemplateEngine.Name))
                    {
                        buildTask.TemplateEngine.Name = Project.TemplateEngine.Name;
                    }

                    if (String.IsNullOrEmpty(buildTask.TemplateEngine.Root))
                    {
                        buildTask.TemplateEngine.Root = Project.TemplateEngine.Root;
                    }

                    if (String.IsNullOrEmpty(buildTask.TemplateEngine.Path))
                    {
                        buildTask.TemplateEngine.Path = Project.TemplateEngine.Path;
                    }
                }

                if (buildTask.NamingConverter == null)
                {
                    buildTask.NamingConverter = Project.NamingConverter;
                }
                else
                {
                    if (buildTask.NamingConverter.Table == null)
                    {
                        buildTask.NamingConverter.Table = Project.NamingConverter.Table;
                    }

                    if (buildTask.NamingConverter.View == null)
                    {
                        buildTask.NamingConverter.View = Project.NamingConverter.View;
                    }

                    if (buildTask.NamingConverter.Column == null)
                    {
                        buildTask.NamingConverter.Column = Project.NamingConverter.Column;
                    }
                }

                if (Project.TableFilter != null)
                {
                    if (buildTask.IgnoreTables == null)
                    {
                        buildTask.IgnoreTables = Project.TableFilter.IgnoreTables;
                    }

                    if (buildTask.IncludeTables == null)
                    {
                        buildTask.IncludeTables = Project.TableFilter.IncludeTables;
                    }

                    if (!buildTask.IgnoreView.HasValue)
                    {
                        buildTask.IgnoreView = Project.TableFilter.IgnoreView;
                    }

                    if (!buildTask.IgnoreNoPKTable.HasValue)
                    {
                        buildTask.IgnoreNoPKTable = Project.TableFilter.IgnoreNoPKTable;
                    }
                }

                if (buildTask.Parameters == null)
                {
                    buildTask.Parameters = new Dictionary<string, object>();
                }
            }
        }
    }
}