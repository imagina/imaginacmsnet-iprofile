namespace Iprofile.Config
{
    public static class Configs
    {

        private static string configs = @"{
            'name':'Iprofile',
            'iprofile':{'partials':{'translatable':{'create':[],'edit':[]},'normal':{'create':[],'edit':[]}},'relations':[]},
            'panel':'blade',
            'crud-fields':{
                'roles':{
                    'externalId': {
                          'name': 'externalId',
                          'value': null,
                          'type': 'input',
                          'props': {
                                'label': 'AD Group Id'
                          },
                    },
                    'offlineRequests': {
                          'name': 'offlineRequests',
                          'value': '',
                          'type': 'json',
                          'isFakeField': true,
                          'props': {
                                'label': 'Offline Request to pre-cach'
                          },
                    }
                },
                'users':{
                    'buildingsAssigned': {
                          'name': 'buildingsAssigned',
                          'value': [],
                          'type': 'select',
                          'isFakeField': true,
                          'loadOptions': {
                                'apiRoute': 'apiRoutes.qramp.setupBuildings',
                                'select': {'label': 'buildingName', 'id': 'id'},
                          },
                          'props': {
                                'label': 'Buildings Assigned',
                                'entityId': null,
                                'multiple': true,
                                'useChips': true,
                                'clearable': true
                          },
                    },
                  
                    'businessUnitType': {
                          'name': 'businessUnitType',
                          'value': null,
                          'type': 'select',
                          'isFakeField': true,
                          'loadOptions': {
                                'apiRoute': 'apiRoutes.qramp.setupBusinessUnitTypes',
                                'select': {'label': 'unitTypeName', 'id': 'id'},
                          },
                          'props': {
                                'label': 'Business Unit Type',
                                'clearable': true
                          },
                    },
                    'stationsAssigned': {
                          'name': 'stationsAssigned',
                          'value': [],
                          'type': 'select',
                          'isFakeField': true,
                          'loadOptions': {
                                'apiRoute': 'apiRoutes.qramp.setupStations',
                                'select': {'label': 'stationName', 'id': 'id'},
                          },
                          'props': {
                                'label': 'Stations Assigned',
                                'entityId': null,
                                'multiple': true,
                                'useChips': true,
                                'clearable': true
                          },
                    },
                    'companyAssigned': {
                          'name': 'companyAssigned',
                          'value': null,
                          'type': 'select',
                          'isFakeField': true,
                          'loadOptions': {
                                'apiRoute': 'apiRoutes.qsetupagione.setupCompanies',
                                'select': {'label': 'companyName', 'id': 'id'},
                          },
                          'props': {
                                'label': 'Company Assigned',
                                'entityId': null,
                                'clearable': true
                          },
                    },
                    'timezone': {
                          'name': 'timezone',
                          'value': null,
                          'type': 'select',
                          'loadOptions': {
                                'apiRoute': 'apiRoutes.qfly.timezones',
                                'select': {'label': 'name', 'id': 'value'},
                          },
                          'props': {
                                'label': 'Timezone',
                                'clearable': true
                          },
                    }
                }
            },
            'exportable':{
                
                'user':{
                    'moduleName':'Iprofile',
                    'fileName':'Users',
                    'fields':['id','first__name','last__name','email','last__login','created__at','updated__at'],
                    'headings':['id','First Name','Last Name','Email','Last Login','Created At','Updated At'],
                    'repositoryName':'UserApiRepository',
                    'formats':['csv'],
                    'apiRoute': '/profile/v1/users/export'
                }
                
            }
        }";

        public static string GetConfigs()
        {
            return configs;
        }
    }
}
