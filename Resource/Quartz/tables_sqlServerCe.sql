/* 

These statements are here for your convenience. Uncomment them if you want to
clear and recreate the database from scratch. If you are creating the database
for the first time, there is no need to uncomment this section.

ALTER TABLE [QRTZ_TRIGGERS] DROP CONSTRAINT FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS
GO

ALTER TABLE [QRTZ_CRON_TRIGGERS] DROP CONSTRAINT FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS
GO

ALTER TABLE [QRTZ_SIMPLE_TRIGGERS] DROP CONSTRAINT FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS
GO

ALTER TABLE [QRTZ_SIMPROP_TRIGGERS] DROP CONSTRAINT FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS
GO

DROP TABLE [QRTZ_CALENDARS]
GO

DROP TABLE [QRTZ_CRON_TRIGGERS]
GO

DROP TABLE [QRTZ_BLOB_TRIGGERS]
GO

DROP TABLE [QRTZ_FIRED_TRIGGERS]
GO

DROP TABLE [QRTZ_PAUSED_TRIGGER_GRPS]
GO

DROP TABLE [QRTZ_SCHEDULER_STATE]
GO

DROP TABLE [QRTZ_LOCKS]
GO

DROP TABLE [QRTZ_JOB_DETAILS]
GO

DROP TABLE [QRTZ_SIMPLE_TRIGGERS]
GO

DROP TABLE QRTZ_SIMPROP_TRIGGERS
GO

DROP TABLE [QRTZ_TRIGGERS]
GO
*/

CREATE TABLE [QRTZ_CALENDARS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [CALENDAR_NAME] [NVARCHAR] (200)  NOT NULL ,
  [CALENDAR] [IMAGE] NOT NULL
) 
GO

CREATE TABLE [QRTZ_CRON_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [CRON_EXPRESSION] [NVARCHAR] (120)  NOT NULL ,
  [TIME_ZONE_ID] [NVARCHAR] (80) 
)
GO

CREATE TABLE [QRTZ_FIRED_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [ENTRY_ID] [NVARCHAR] (95)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [INSTANCE_NAME] [NVARCHAR] (200)  NOT NULL ,
  [FIRED_TIME] [BIGINT] NOT NULL ,
  [PRIORITY] [INTEGER] NOT NULL ,
  [STATE] [NVARCHAR] (16)  NOT NULL,
  [JOB_NAME] [NVARCHAR] (150)  NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NULL ,
  [IS_NONCONCURRENT] [NVARCHAR] (1)  NULL ,
  [REQUESTS_RECOVERY] [NVARCHAR] (1)  NULL 
)
GO

CREATE TABLE [QRTZ_PAUSED_TRIGGER_GRPS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL 
)
GO

CREATE TABLE [QRTZ_SCHEDULER_STATE] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [INSTANCE_NAME] [NVARCHAR] (200)  NOT NULL ,
  [LAST_CHECKIN_TIME] [BIGINT] NOT NULL ,
  [CHECKIN_INTERVAL] [BIGINT] NOT NULL
)
GO

CREATE TABLE [QRTZ_LOCKS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [LOCK_NAME] [NVARCHAR] (40)  NOT NULL 
)
GO

CREATE TABLE [QRTZ_JOB_DETAILS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [JOB_NAME] [NVARCHAR] (150)  NOT NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [DESCRIPTION] [NVARCHAR] (250) NULL ,
  [JOB_CLASS_NAME] [NVARCHAR] (250)  NOT NULL ,
  [IS_DURABLE] [NVARCHAR] (1)  NOT NULL ,
  [IS_NONCONCURRENT] [NVARCHAR] (1)  NOT NULL ,
  [IS_UPDATE_DATA] [NVARCHAR] (1)  NOT NULL ,
  [REQUESTS_RECOVERY] [NVARCHAR] (1)  NOT NULL ,
  [JOB_DATA] [IMAGE] NULL
)
GO

CREATE TABLE [QRTZ_SIMPLE_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [REPEAT_COUNT] [INTEGER] NOT NULL ,
  [REPEAT_INTERVAL] [BIGINT] NOT NULL ,
  [TIMES_TRIGGERED] [INTEGER] NOT NULL
)
GO

CREATE TABLE [QRTZ_SIMPROP_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [STR_PROP_1] [NVARCHAR] (512) NULL,
  [STR_PROP_2] [NVARCHAR] (512) NULL,
  [STR_PROP_3] [NVARCHAR] (512) NULL,
  [INT_PROP_1] [INT] NULL,
  [INT_PROP_2] [INT] NULL,
  [LONG_PROP_1] [BIGINT] NULL,
  [LONG_PROP_2] [BIGINT] NULL,
  [DEC_PROP_1] [NUMERIC] (13,4) NULL,
  [DEC_PROP_2] [NUMERIC] (13,4) NULL,
  [BOOL_PROP_1] [NVARCHAR] (1) NULL,
  [BOOL_PROP_2] [NVARCHAR] (1) NULL
)
GO

CREATE TABLE [QRTZ_BLOB_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [BLOB_DATA] [IMAGE] NULL
)
GO

CREATE TABLE [QRTZ_TRIGGERS] (
  [SCHED_NAME] [NVARCHAR] (100)  NOT NULL ,
  [TRIGGER_NAME] [NVARCHAR] (150)  NOT NULL ,
  [TRIGGER_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [JOB_NAME] [NVARCHAR] (150)  NOT NULL ,
  [JOB_GROUP] [NVARCHAR] (150)  NOT NULL ,
  [DESCRIPTION] [NVARCHAR] (250) NULL ,
  [NEXT_FIRE_TIME] [BIGINT] NULL ,
  [PREV_FIRE_TIME] [BIGINT] NULL ,
  [PRIORITY] [INTEGER] NULL ,
  [TRIGGER_STATE] [NVARCHAR] (16)  NOT NULL ,
  [TRIGGER_TYPE] [NVARCHAR] (8)  NOT NULL ,
  [START_TIME] [BIGINT] NOT NULL ,
  [END_TIME] [BIGINT] NULL ,
  [CALENDAR_NAME] [NVARCHAR] (200)  NULL ,
  [MISFIRE_INSTR] [INTEGER] NULL ,
  [JOB_DATA] [IMAGE] NULL
)
GO

ALTER TABLE [QRTZ_CALENDARS]  ADD
  CONSTRAINT [PK_QRTZ_CALENDARS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [CALENDAR_NAME]
  ) 
GO

ALTER TABLE [QRTZ_CRON_TRIGGERS] ADD
  CONSTRAINT [PK_QRTZ_CRON_TRIGGERS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_FIRED_TRIGGERS] ADD
  CONSTRAINT [PK_QRTZ_FIRED_TRIGGERS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [ENTRY_ID]
  ) 
GO

ALTER TABLE [QRTZ_PAUSED_TRIGGER_GRPS] ADD
  CONSTRAINT [PK_QRTZ_PAUSED_TRIGGER_GRPS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_SCHEDULER_STATE] ADD
  CONSTRAINT [PK_QRTZ_SCHEDULER_STATE] PRIMARY KEY 
  (
    [SCHED_NAME],
    [INSTANCE_NAME]
  ) 
GO

ALTER TABLE [QRTZ_LOCKS] ADD
  CONSTRAINT [PK_QRTZ_LOCKS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [LOCK_NAME]
  ) 
GO

ALTER TABLE [QRTZ_JOB_DETAILS] ADD
  CONSTRAINT [PK_QRTZ_JOB_DETAILS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_SIMPLE_TRIGGERS] ADD
  CONSTRAINT [PK_QRTZ_SIMPLE_TRIGGERS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_SIMPROP_TRIGGERS] ADD
  CONSTRAINT [PK_QRTZ_SIMPROP_TRIGGERS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_TRIGGERS] ADD
  CONSTRAINT [PK_QRTZ_TRIGGERS] PRIMARY KEY 
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) 
GO

ALTER TABLE [QRTZ_CRON_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_CRON_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [QRTZ_SIMPLE_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_SIMPLE_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [QRTZ_SIMPROP_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_SIMPROP_TRIGGERS_QRTZ_TRIGGERS] FOREIGN KEY
  (
	[SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) REFERENCES [QRTZ_TRIGGERS] (
    [SCHED_NAME],
    [TRIGGER_NAME],
    [TRIGGER_GROUP]
  ) ON DELETE CASCADE
GO

ALTER TABLE [QRTZ_TRIGGERS] ADD
  CONSTRAINT [FK_QRTZ_TRIGGERS_QRTZ_JOB_DETAILS] FOREIGN KEY
  (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  ) REFERENCES [QRTZ_JOB_DETAILS] (
    [SCHED_NAME],
    [JOB_NAME],
    [JOB_GROUP]
  )
GO

