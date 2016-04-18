// ReSharper disable UnassignedReadonlyField

using EFSession.Session;

namespace EFSession.StoredProcedures
{
    public class StoredProceduresContainer
    {
        public static StoredProceduresContainer Instance { get; }

        static StoredProceduresContainer()
        {
            Instance = new StoredProceduresContainer();
        }

        [HasSchemaParameter]
        public readonly string SpFindAgency;

        [HasSchemaParameter]
        public readonly string SpListView;

        [HasSchemaParameter]
        public readonly string SpGetLocationsByFolder;

        [HasSchemaParameter]
        public readonly string SpGetFolderDistribution;

        [HasSchemaParameter]
        public readonly string SpReviewsDistribution;

        [HasSchemaParameter]
        public readonly string SpAgencyGoogleOrganicSearchDistribution;

        [HasSchemaParameter]
        public readonly string SpDataQualityDistribution;

        [HasSchemaParameter]
        public readonly string SpBrandScoreRecentActivity;

        [HasSchemaParameter]
        public readonly string SpDataQualityRecentActivity;

        [HasSchemaParameter]
        public readonly string SpDataQualityRecentActivityLocationDatabase;

        [HasSchemaParameter]
        public readonly string SpSearchRecentActivity;

        [HasSchemaParameter]
        public readonly string SpEngagementLocatorDataByDate;

        [HasSchemaParameter]
        public readonly string SpEngagementSocialDataByDateRange;

        [HasSchemaParameter]
        public readonly string SpSeoData;

        [HasSchemaParameter]
        public readonly string SpLocationsShortInfo;

        [HasSchemaParameter]
        public readonly string SpGetRecentInsightsShortInfo;

        [HasSchemaParameter]
        public readonly string SpGetRecentReviewsCount;

        [HasSchemaParameter]
        public readonly string SpGetTotalInsightsShortInfo;

        [HasSchemaParameter]
        public readonly string SpGetReviewBySolrId;

        [HasSchemaParameter]
        public readonly string SpReviewsByThreshold;

        [HasSchemaParameter]
        public readonly string SpGetGoogleOrganicInsightsSearchData;

        [HasSchemaParameter]
        public readonly string SpGetGoogleOrganicHistoricalInsightsSearchData;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleAnalyticsLocationDistribution;

        [HasSchemaParameter]
        public readonly string SpRespondedReviews;

        [HasSchemaParameter]
        public readonly string SpGetAnalyticData;

        [HasSchemaParameter]
        public readonly string SpGetPublishDestinationDetails;

        [HasSchemaParameter]
        public readonly string SpGetCompanyPublicationsByDateRange;

        [HasSchemaParameter]
        public readonly string SpBrandScoreTimeline;

        [HasSchemaParameter]
        public readonly string SpDataQualitySummary;

        [HasSchemaParameter]
        public readonly string SpSocialTimeline;

        [HasSchemaParameter]
        public readonly string SpSearchTimeline;

        public readonly string SpGetThresholdReviewBounds;

        [HasSchemaParameter]
        public readonly string SpMapScoreToEnum;

        [HasSchemaParameter]
        public readonly string SpAgencyLocationsScoreReport;

        [HasSchemaParameter]
        public readonly string SpSynchronizeLocationsCategories;

        [HasSchemaParameter]
        public readonly string SpGoogleAnalyticsDistribution;

        [HasSchemaParameter]
        public readonly string SpGALocationDistribution;

        [HasSchemaParameter]
        public readonly string SpListViewBrandscoreRecentActivity;

        [HasSchemaParameter]
        public readonly string SpGetListView;

        [HasSchemaParameter]
        public readonly string SpListViewDataQuality;

        [HasSchemaParameter]
        public readonly string SpListViewSearchDistribution;

        [HasSchemaParameter]
        public readonly string SpListViewReviewsRecentActivity;

        [HasSchemaParameter]
        public readonly string SpListViewReviewKeywords;

        [HasSchemaParameter]
        public readonly string SpListViewEngagementLocator;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleAnalyticsAllTraffic;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleAnalyticsDirect;

        [HasSchemaParameter]
        public readonly string SpListViewDataQualityRecentActivity;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleAnalyticsDistribution;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleAnalyticsOther;

        [HasSchemaParameter]
        public readonly string SpListViewGoogleOrganicSEO;

        [HasSchemaParameter]
        public readonly string SpListViewEngagementSocialByDateRange;

        [HasSchemaParameter]
        public readonly string SpReviewDistributionListView;

        [HasSchemaParameter]
        public readonly string SpListingsReport;

        [HasSchemaParameter]
        public readonly string SpOnlineListingDetails;

        [HasSchemaParameter]
        public readonly string SpBrandScoreDistribution;

        [HasSchemaParameter]
        public readonly string SpCompanyReviewsRecentActivityRatingsDistribution;

        [HasSchemaParameter]
        public readonly string SpSatisfactionScoreByFilter;

        [HasSchemaParameter]
        public readonly string SpUpdateRanks;

        [HasSchemaParameter]
        public readonly string SpGetReviewsForMatching;

        [HasSchemaParameter]
        public readonly string SpReviewsRequireResponses;

        [HasSchemaParameter]
        public readonly string SpInsightsSearchDistribution;

        [HasSchemaParameter]
        public readonly string SpW2GISyncDetails;

        [HasSchemaParameter]
        public readonly string SpReviewsRecentActivityProvidersDistribution;

        [HasSchemaParameter]
        public readonly string SpGetReviewKeywordDistribution;

        [HasSchemaParameter]
        public readonly string SpListViewSetElements;

        public readonly string SpGetAgencyAvailableFeatures;
        
        [HasSchemaParameter]
        public readonly string SpReviewTimeline;

        [HasSchemaParameter]
        public readonly string SpGetAgencyProvidersAverageRating;

        public readonly string SpResolveSchemaByCompanyId;

        public readonly string SpResolveSchemaByAgencyId;

        public readonly string SpResolveSchemaByUserId;
        
        [HasSchemaParameter]
        public readonly string SpGetTimelineReviewsByDateRange;

        [HasSchemaParameter]
        public readonly string SpAllReviews;

        [HasSchemaParameter]
        public readonly string SpReplyRequiredReviews;

        [HasSchemaParameter]
        public readonly string SpReplyPostedReviews;

        [HasSchemaParameter]
        public readonly string SpDataQualityTimelineEnhanced;

        [HasSchemaParameter]
        public readonly string SpGoogleAnalyticsTimeline;

        [HasSchemaParameter]
        public readonly string SpGoogleAnalyticsLocationTimeline;

        [HasSchemaParameter]
        public readonly string SpGoogleAnalyticsDates;

        [HasSchemaParameter]
        public readonly string SpGoogleOrganicTimeline;

        [HasSchemaParameter]
        public readonly string SpLocatorTimeline;

        [HasSchemaParameter]
        public readonly string SpSocialTimelineByDateRange;

        [HasSchemaParameter]
        public readonly string SpGetUserLocations;

        [HasSchemaParameter]
        public readonly string SpCheckAndFixDataQualityLastPoint;

        [HasSchemaParameter]
        public readonly string SpDataQualityListingDetails;

        [HasSchemaParameter]
        public readonly string SpAgencyExploreView;

        [HasSchemaParameter]
        public readonly string SpLocationsPublishInfo;

        [HasSchemaParameter]
        public readonly string SpClearGoogleAnalyticsData;

        [HasSchemaParameter]
        public readonly string SpFootPrintBuildHistory;

        [HasSchemaParameter]
        public readonly string SpAgencyNextRefreshDates;

        [HasSchemaParameter]
        public readonly string SpGetLatestAgencyRefreshesStatus;

        [HasSchemaParameter]
        public readonly string SpUpdateGoogleReviewsAuthorAgencyAll;

        [HasSchemaParameter]
        public readonly string SpMigrateMissedReviewsFromDbToSolr;

        [HasSchemaParameter]
        public readonly string SpGetAgencyPublishmentDate;
    }
}