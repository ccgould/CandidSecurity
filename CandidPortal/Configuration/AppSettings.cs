using System;
using System.Collections.Generic;
using System.Configuration;


namespace CandidPortal.Configuration;
public class AppSettings : ConfigurationSection
{
    private List<string> additionalDirectories;

    [ConfigurationProperty("mySqlUsername", DefaultValue = "root")]
    public string MySqlUsername
    {
        get { return (string)this["mySqlUsername"]; }
        set { this["mySqlUsername"] = value; }
    }

    [ConfigurationProperty("mysqlDatabaseName", DefaultValue = "candid_db")]
    public string MysqlDatabaseName
    {
        get { return (string)this["mysqlDatabaseName"]; }
        set { this["mysqlDatabaseName"] = value; }
    }

    [ConfigurationProperty("mysqlDatasource", DefaultValue = "localhost")]
    public string MysqlDatasource
    {
        get { return (string)this["mysqlDatasource"]; }
        set { this["mysqlDatasource"] = value; }
    }

    [ConfigurationProperty("mysqlPassword", DefaultValue = "Candid@5499")]
    public string MysqlPassword
    {
        get { return (string)this["mysqlPassword"]; }
        set { this["mysqlPassword"] = value; }
    }

    [ConfigurationProperty("mysqlPortNumber", DefaultValue = 3306)]
    public int MysqlPortNumber
    {
        get { return (int)this["mysqlPortNumber"]; }
        set { this["mysqlPortNumber"] = value; }
    }
}