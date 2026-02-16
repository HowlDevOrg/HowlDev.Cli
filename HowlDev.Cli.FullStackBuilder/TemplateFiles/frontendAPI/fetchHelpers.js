const baseUrl = "/api";

async function request(method, path, body, basePath) {
  const startUrl = basePath ?? baseUrl;
  const fullUrl = startUrl + path;

  console.log(`[REQUEST START] ${method} ${fullUrl}`, body ? { body } : "");

  const headers = {};

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

export async function getResponse(path, basePath) {
  return request("GET", path, undefined, basePath);
}

export async function postResponse(path, body, basePath) {
  return request("POST", path, body, basePath);
}

export async function patchResponse(path, body, basePath) {
  return request("PATCH", path, body, basePath);
}

export async function deleteResponse(path, basePath) {
  return request("DELETE", path, undefined, basePath);
}
