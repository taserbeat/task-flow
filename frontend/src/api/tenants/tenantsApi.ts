import type { AxiosInstance } from "axios";

import type {
  CreateTenantRequest,
  TenantDetailResponse,
  TenantSummaryResponse,
  UpdateTenantRequest,
} from "./tenantsDtos";
import type { TenantSummary } from "../../models/tenants/TenantSummary";
import { toTenantDetail, toTenantSummary } from "./tenantsMapper";
import type { TenantDetail } from "../../models/tenants/TenantDetail";

export class TenantsApi {
  private readonly instance: AxiosInstance;

  public constructor(instance: AxiosInstance) {
    this.instance = instance;
  }

  /** テナントを作成する */
  public async createTenant(request: CreateTenantRequest): Promise<void> {
    const url = "/api/tenants";
    await this.instance.post(url, {
      ...request,
    });
  }

  /** テナントの一覧を取得する */
  public async getTenants(): Promise<TenantSummary[]> {
    const url = "/api/tenants";
    const response = await this.instance.get<TenantSummaryResponse[]>(url);

    return response.data.map((x) => toTenantSummary(x));
  }

  /** テナントの詳細情報を取得する */
  public async getTenant(tenantId: string): Promise<TenantDetail> {
    const url = `/api/tenants/${tenantId}`;
    const response = await this.instance.get<TenantDetailResponse>(url);

    return toTenantDetail(response.data);
  }

  /** テナントを更新する */
  public async updateTenant(
    tenantId: string,
    request: UpdateTenantRequest,
  ): Promise<void> {
    const url = `/api/tenants/${tenantId}`;
    await this.instance.put(url, {
      ...request,
    });
  }

  /** テナントを削除する */
  public async deleteTenant(tenantId: string): Promise<void> {
    const url = `/api/tenants/${tenantId}`;
    await this.instance.delete(url);
  }
}
