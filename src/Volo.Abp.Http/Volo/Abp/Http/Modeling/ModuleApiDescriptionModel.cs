﻿using System;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;

namespace Volo.Abp.Http.Modeling
{
    [Serializable]
    public class ModuleApiDescriptionModel
    {
        /// <summary>
        /// "app".
        /// </summary>
        public const string DefaultRootPath = "app";

        public string RootPath { get; set; }

        public IDictionary<string, ControllerApiDescriptionModel> Controllers { get; set; }

        private ModuleApiDescriptionModel()
        {
            
        }

        public static ModuleApiDescriptionModel Create(string rootPath)
        {
            return new ModuleApiDescriptionModel
            {
                RootPath = rootPath,
                Controllers = new Dictionary<string, ControllerApiDescriptionModel>()
            };
        }

        public ControllerApiDescriptionModel AddController(ControllerApiDescriptionModel controller)
        {
            if (Controllers.ContainsKey(controller.ControllerName))
            {
                throw new AbpException($"There is already a controller with name: {controller.ControllerName} in module: {RootPath}");
            }

            return Controllers[controller.ControllerName] = controller;
        }

        public ControllerApiDescriptionModel GetOrAddController(string uniqueName, string name, Type type, [CanBeNull] HashSet<Type> ignoredInterfaces = null)
        {
            return Controllers.GetOrAdd(uniqueName, () => ControllerApiDescriptionModel.Create(name, type, ignoredInterfaces));
        }
        
        public ModuleApiDescriptionModel CreateSubModel(string[] controllers, string[] actions)
        {
            var subModel = Create(RootPath);

            foreach (var controller in Controllers.Values)
            {
                if (controllers == null || controllers.Contains(controller.ControllerName))
                {
                    subModel.AddController(controller.CreateSubModel(actions));
                }
            }

            return subModel;
        }
    }
}