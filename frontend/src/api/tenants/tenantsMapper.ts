import type { TenantDetail } from "../../models/tenants/TenantDetail";
import type { TenantSummary } from "../../models/tenants/TenantSummary";
import type {
  TenantDetailResponse,
  TenantSummaryResponse,
} from "./tenantsDtos";

export function toTenantSummary(dto: TenantSummaryResponse): TenantSummary {
  return dto;
}

export function toTenantDetail(dto: TenantDetailResponse): TenantDetail {
  return dto;
}
