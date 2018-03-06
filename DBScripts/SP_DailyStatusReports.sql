USE [Reactions]
GO



/****** Object:  StoredProcedure [dbo].[DailyStatus]    Script Date: 13-Dec-2017 09:43:04 AM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

CREATE procedure [dbo].[DailyStatus]
@fromdate datetime,
@todate datetime,
@username varchar(200)
as
--declare @fromdate datetime
--declare @todate datetime
--declare @username varchar(200)
--set @fromdate=GETDATE()-22
--set @todate=GETDATE()-21
--set @username='lavanya.siluveru'
--select @fromdate,@todate
--create table data(tanid int,CuratorDate date,ReviewDate date,QCDate date)

insert into dbo.data
select tanid,cast(a.LastUpdatedDate as date) as CuratorDate, cast(a.ReviewLastUpdatedDate as date) as ReviewDate, cast(a.QCLastUpdatedDate as date) as QCDate
from (select 
JSON_VALUE(Reactions.Value,'$.Id') REACTIONID,
JSON_VALUE(Reactions.Value,'$.Name') NAME,
JSON_VALUE(Reactions.Value,'$.TanId') TanID,
JSON_VALUE(Reactions.Value,'$.AnalogousFromId') AnalogousFormID,
JSON_VALUE(Reactions.Value,'$.LastUpdatedDate') LastUpdatedDate,
JSON_VALUE(Reactions.Value,'$.DisplayOrder') DisplayOrder,
JSON_VALUE(Reactions.Value,'$.Yield') Yield,
JSON_VALUE(Reactions.Value,'$.IsCurationCompleted') IsCurationCompleted,
JSON_VALUE(Reactions.Value,'$.IsReviewCompleted') IsReviewCompleted,
cast(JSON_VALUE(Reactions.Value,'$.ReviewLastUpdatedDate') as datetime2) ReviewLastUpdatedDate,
cast(JSON_VALUE(Reactions.Value,'$.QcLastUpdatedDate')as datetime2) QCLastUpdatedDate
from TanDatas
cross apply openjson(CAST(Data as nvarchar(max)),'$.Reactions')as Reactions
where ISJSON( CAST(Data as nvarchar(max)))>0 
--and TanID in (select id from Tans where CuratorId=(select id from AspNetUsers where UserName=@username))  
) as a
--where datediff(day,@fromdate,a.LastUpdatedDate)>=0 and datediff(day,@todate,a.lastUpdatedDate)<=0
--group by cast(a.LastUpdatedDate as date),TanID
 
 

select a.username,count(*) CuratedReactions,CuratorDate,'Curator' Role from AspNetUsers  a
inner join 
tans b on a.id=b.CuratorId
inner join 
dbo.data d on d.tanid=b.id
where datediff(day,@fromdate,d.Curatordate)>=0 and datediff(day,@todate,d.curatordate)<=0
group by d.curatordate,a.UserName
union 
select a.username,count(*) CuratedReactions,ReviewDate,'Reviewer' Role from AspNetUsers  a
inner join 
tans b on a.id=b.ReviewerId
inner join 
dbo.data d on d.tanid=b.id
where datediff(day,@fromdate,d.ReviewDate)>=0 and datediff(day,@todate,d.ReviewDate)<=0
group by d.ReviewDate,a.UserName
union
select a.username,count(*) CuratedReactions,QCDate,'QC' Role from AspNetUsers  a
inner join 
tans b on a.id=b.QCId
inner join 
dbo.data d on d.tanid=b.id
where datediff(day,@fromdate,d.QCDate)>=0 and datediff(day,@todate,d.QCDate)<=0
group by d.QCDate,a.UserName
order by a.UserName
--where username='lavanya.siluveru'

delete from dbo.data

GO


