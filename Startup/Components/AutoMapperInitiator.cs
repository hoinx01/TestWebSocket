using System;
using System.Collections.Generic;
using System.Text;
using AutoMapper;

namespace Startup.Components
{
    public static class AutoMapperInitiator
    {
        public static void Init()
        {
            Mapper.Initialize((cfg) => {
                cfg.CreateMissingTypeMaps = true;
                cfg.ForAllMaps((map, exp) => {
                    foreach (var unmappedPropertyName in map.GetUnmappedPropertyNames())
                    {
                        exp.ForMember(unmappedPropertyName, opt => opt.Ignore());
                    }
                });
            });
            RegisterMapping();
        }
        /*
         * Cần custom mapping nào thì viết ở đây
         */
        public static void RegisterMapping()
        {
            return;
        }
    }
}
