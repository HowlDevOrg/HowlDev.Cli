export interface ApiResponse {
  code: number;
  response: string;
}

const baseUrl = "/api";

async function request(method: "GET" | "POST" | "PATCH" | "DELETE", path: string, body?: object, basePath?: string) {
  const startUrl = basePath ?? baseUrl;
  const fullUrl = startUrl + path;

  console.log(`[REQUEST START] ${method} ${fullUrl}`, body ? { body } : "");

  const headers: Record<string, string> = {};

  if (body != undefined) {
    headers["Content-Type"] = "application/json";
  }

  const response = await fetch(fullUrl, {
    method,
    headers,
    body: body !== undefined ? JSON.stringify(body) : undefined,
  });

  const text = await response.text();

  console.log(`[RESPONSE RECEIVED] ${method} ${fullUrl} - Status: ${response.status}`, text);

  if (response.status >= 500) {
    console.error(`Server error ${response.status}: ${text}`);
    throw new Error(`HTTP ${response.status}: ${text}`);
  }

  if (!response.ok) {
    console.error(`HTTP ${response.status}: ${text}`);
    throw new Error(`HTTP ${response.status}: ${text}`);
  }

  return { code: response.status, response: text };
}

export async function getResponse(path: string, basePath?: string): Promise<ApiResponse> {
  return request("GET", path, undefined, basePath);
}

export async function postResponse(path: string, body?: object, basePath?: string): Promise<ApiResponse> {
  return request("POST", path, body, basePath);
}

export async function patchResponse(path: string, body?: object, basePath?: string): Promise<ApiResponse> {
  return request("PATCH", path, body, basePath);
}

export async function deleteResponse(path: string, basePath?: string): Promise<ApiResponse> {
  return request("DELETE", path, undefined, basePath);
}
